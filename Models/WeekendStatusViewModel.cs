namespace hotzerver.pilkki.calendar.Models;

public class WeekendStatusViewModel
{
    public required DateOnly Friday { get; set; }
    public required DateOnly Sunday { get; set; }
    public List<string> HolidayTitles { get; set; } = [];
    public required List<ParticipantStatusViewModel> Participants { get; set; }
}

public class ParticipantStatusViewModel
{
    public required int ParticipantId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public bool IsAvailable { get; set; }
    public string? Comment { get; set; }
}
