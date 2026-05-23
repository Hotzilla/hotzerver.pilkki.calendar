namespace hotzerver.pilkki.calendar.Models;

public class SelectedWeekend
{
    public int Id { get; set; }
    public int Year { get; set; }
    public TripSeason Season { get; set; }
    public DateOnly WeekendStart { get; set; }
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
