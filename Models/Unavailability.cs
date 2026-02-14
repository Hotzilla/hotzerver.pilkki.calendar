namespace hotzerver.pilkki.calendar.Models;

public class Unavailability
{
    public int Id { get; set; }
    public int ParticipantId { get; set; }
    public Participant Participant { get; set; } = default!;
    public int Year { get; set; }
    public TripSeason Season { get; set; }
    public DateOnly WeekendStart { get; set; }
    public string? Comment { get; set; }
    public UnavailabilityPriority Priority { get; set; } = UnavailabilityPriority.MaybeNegotiable;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
