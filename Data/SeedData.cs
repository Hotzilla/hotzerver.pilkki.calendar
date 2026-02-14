using hotzerver.pilkki.calendar.Models;

namespace hotzerver.pilkki.calendar.Data;

public static class SeedData
{
    public static void EnsureSeeded(PilkkiDbContext db)
    {
        if (db.Participants.Any())
        {
            return;
        }

        var participants = new[]
        {
            new Participant { FirstName = "Tuukka", LastName = "DEMO" },
            new Participant { FirstName = "Sami", LastName = "DEMO" },
            new Participant { FirstName = "Kristian", LastName = "DEMO" },
            new Participant { FirstName = "Keijo", LastName = "DEMO" },
            new Participant { FirstName = "Petri", LastName = "DEMO" },
            new Participant { FirstName = "Miika", LastName = "DEMO" },
            new Participant { FirstName = "Hannu", LastName = "DEMO" },
            new Participant { FirstName = "Janne", LastName = "DEMO" },
            new Participant { FirstName = "Veli-Matti", LastName = "DEMO" }
        };

        db.Participants.AddRange(participants);
        db.SaveChanges();
    }
}
