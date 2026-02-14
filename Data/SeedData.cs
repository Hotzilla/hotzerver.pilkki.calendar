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
            new Participant { FirstName = "Antti", LastName = "Aalto" },
            new Participant { FirstName = "Mikko", LastName = "Mäkinen" },
            new Participant { FirstName = "Janne", LastName = "Järvinen" },
            new Participant { FirstName = "Teemu", LastName = "Tuominen" },
            new Participant { FirstName = "Pasi", LastName = "Paananen" },
            new Participant { FirstName = "Sami", LastName = "Salonen" },
            new Participant { FirstName = "Timo", LastName = "Tikkanen" },
            new Participant { FirstName = "Jari", LastName = "Jokinen" },
            new Participant { FirstName = "Kari", LastName = "Kettunen" },
            new Participant { FirstName = "Petri", LastName = "Pitkänen" }
        };

        db.Participants.AddRange(participants);
        db.SaveChanges();
    }
}
