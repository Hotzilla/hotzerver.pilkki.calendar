using Microsoft.EntityFrameworkCore;
using hotzerver.pilkki.calendar.Models;

namespace hotzerver.pilkki.calendar.Data;

public class PilkkiDbContext(DbContextOptions<PilkkiDbContext> options) : DbContext(options)
{
    public DbSet<Participant> Participants => Set<Participant>();
    public DbSet<Unavailability> Unavailabilities => Set<Unavailability>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Participant>()
            .HasIndex(p => new { p.FirstName, p.LastName })
            .IsUnique();

        modelBuilder.Entity<Unavailability>()
            .HasIndex(u => new { u.ParticipantId, u.Year, u.Season, u.WeekendStart })
            .IsUnique();

        modelBuilder.Entity<Unavailability>()
            .HasOne(u => u.Participant)
            .WithMany()
            .HasForeignKey(u => u.ParticipantId);

        modelBuilder.Entity<Unavailability>()
            .Property(u => u.Priority)
            .HasDefaultValue(UnavailabilityPriority.MaybeNegotiable);
    }
}
