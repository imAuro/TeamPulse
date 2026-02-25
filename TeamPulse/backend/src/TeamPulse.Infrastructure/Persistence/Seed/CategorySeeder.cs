using Microsoft.EntityFrameworkCore;
using TeamPulse.Domain.Pulses;

namespace TeamPulse.Infrastructure.Persistence.Seed;

public static class CategorySeeder
{
    public static async Task SeedAsync(PulseDbContext db, CancellationToken ct)
    {
        if (await db.PulseCategories.AnyAsync(ct)) return;

        var cats = new[]
        {
            new PulseCategory(Guid.NewGuid(), "Workload"),
            new PulseCategory(Guid.NewGuid(), "Collaboration"),
            new PulseCategory(Guid.NewGuid(), "Wellbeing"),
            new PulseCategory(Guid.NewGuid(), "Team Dynamics"),
            new PulseCategory(Guid.NewGuid(), "Environment"),
        };

        db.PulseCategories.AddRange(cats);
        await db.SaveChangesAsync(ct);
    }
}