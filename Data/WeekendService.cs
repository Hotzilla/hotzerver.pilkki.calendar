using Microsoft.EntityFrameworkCore;
using hotzerver.pilkki.calendar.Models;

namespace hotzerver.pilkki.calendar.Data;

public class WeekendService(PilkkiDbContext db, HolidayService holidayService)
{
    public async Task<List<Participant>> GetParticipantsAsync()
    {
        return await db.Participants.OrderBy(x => x.FirstName).ToListAsync();
    }

    public async Task<List<WeekendStatusViewModel>> GetWeekendStatusesAsync(int year, TripSeason season)
    {
        var weekends = BuildWeekends(year, season);
        var deadlineDate = CalculateDeadlineDate(weekends);
        var isDeadlineReached = DateOnly.FromDateTime(DateTime.UtcNow) >= deadlineDate;
        var participants = await db.Participants.OrderBy(x => x.FirstName).ToListAsync();
        var unavailable = await db.Unavailabilities
            .Where(x => x.Year == year && x.Season == season)
            .ToListAsync();
        var holidays = await holidayService.GetHolidaysAsync();

        var result = new List<WeekendStatusViewModel>();
        foreach (var friday in weekends)
        {
            var sunday = friday.AddDays(2);
            var statuses = participants.Select(p =>
            {
                var un = unavailable.FirstOrDefault(x => x.ParticipantId == p.Id && x.WeekendStart == friday);
                return new ParticipantStatusViewModel
                {
                    ParticipantId = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    IsAvailable = un is null,
                    Comment = un?.Comment,
                    Priority = un?.Priority ?? UnavailabilityPriority.MaybeNegotiable
                };
            }).ToList();
            var notOkCount = statuses.Count(x => !x.IsAvailable);

            result.Add(new WeekendStatusViewModel
            {
                Friday = friday,
                Sunday = sunday,
                DeadlineDate = deadlineDate,
                IsDeadlineReached = isDeadlineReached,
                NotOkCount = notOkCount,
                HolidayTitles = holidays
                    .Where(h => h.StartDate <= sunday && (h.EndDate ?? h.StartDate) >= friday)
                    .Select(h => h.Title)
                    .Distinct()
                    .ToList(),
                Participants = statuses
            });
        }

        return result;
    }

    public async Task<string?> SetAvailabilityAsync(
        int participantId,
        int year,
        TripSeason season,
        DateOnly weekendStart,
        bool isAvailable,
        string lastname,
        string? comment,
        UnavailabilityPriority priority)
    {
        var participant = await db.Participants.FirstOrDefaultAsync(x => x.Id == participantId);
        if (participant is null)
        {
            return "Osallistujaa ei löytynyt.";
        }

        if (!string.Equals(participant.LastName.Trim(), lastname.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return "Sukunimi ei täsmää.";
        }

        var deadlineDate = CalculateDeadlineDate(BuildWeekends(year, season));
        if (DateOnly.FromDateTime(DateTime.UtcNow) >= deadlineDate)
        {
            return $"Määräaika ({deadlineDate:dd.MM.yyyy}) on umpeutunut. Viikonloppuja ei voi enää muokata.";
        }

        var existing = await db.Unavailabilities.FirstOrDefaultAsync(x =>
            x.ParticipantId == participantId &&
            x.Year == year &&
            x.Season == season &&
            x.WeekendStart == weekendStart);

        if (isAvailable)
        {
            if (existing is not null)
            {
                db.Unavailabilities.Remove(existing);
                await db.SaveChangesAsync();
            }

            return null;
        }

        if (existing is null)
        {
            db.Unavailabilities.Add(new Unavailability
            {
                ParticipantId = participantId,
                Year = year,
                Season = season,
                WeekendStart = weekendStart,
                Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim(),
                Priority = priority,
                UpdatedAtUtc = DateTime.UtcNow
            });
        }
        else
        {
            existing.Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
            existing.Priority = priority;
            existing.UpdatedAtUtc = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();
        return null;
    }

    private static List<DateOnly> BuildWeekends(int year, TripSeason season)
    {
        var start = season == TripSeason.PilkkiI
            ? new DateOnly(year, 2, 1)
            : new DateOnly(year, 9, 1);

        var end = season == TripSeason.PilkkiI
            ? new DateOnly(year, 4, 30)
            : new DateOnly(year, 11, 30);

        while (start.DayOfWeek != DayOfWeek.Friday)
        {
            start = start.AddDays(1);
        }

        var list = new List<DateOnly>();
        for (var current = start; current <= end; current = current.AddDays(7))
        {
            list.Add(current);
        }

        return list;
    }

    private static DateOnly CalculateDeadlineDate(List<DateOnly> weekends)
    {
        var firstWeekend = weekends.Min();
        return firstWeekend.AddMonths(-5);
    }
}
