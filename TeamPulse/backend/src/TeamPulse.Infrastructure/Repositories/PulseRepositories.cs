using Microsoft.EntityFrameworkCore;
using TeamPulse.Application.Common.Abstractions;
using TeamPulse.Application.Pulses.GetPulseSummary;
using TeamPulse.Domain.Pulses;
using TeamPulse.Infrastructure.Persistence;

namespace TeamPulse.Infrastructure.Repositories;

public sealed class PulseRepository(PulseDbContext db) : IPulseRepository
{
    public Task<bool> CategoryExistsAsync(Guid categoryId, CancellationToken ct)
        => db.PulseCategories.AnyAsync(c => c.Id == categoryId, ct);

    public async Task<IReadOnlyList<PulseCategory>> GetCategoriesAsync(CancellationToken ct)
        => await db.PulseCategories.AsNoTracking().OrderBy(c => c.Name).ToListAsync(ct);

    public async Task AddPulseAsync(PulseEntry entry, CancellationToken ct)
    {
        db.PulseEntries.Add(entry);
        await db.SaveChangesAsync(ct);
    }

    public async Task<PulseSummaryReadModel> GetSummaryAsync(CancellationToken ct)
    {
        var entries = db.PulseEntries.AsNoTracking();
        var categories = db.PulseCategories.AsNoTracking();

        var count = await entries.CountAsync(ct);

        decimal avg = 0m;
        if (count > 0)
            avg = await entries.AverageAsync(e => (decimal)e.Score, ct);

        // Ensure scores 1..5 always present
        var scoreCounts = await entries
            .GroupBy(e => e.Score)
            .Select(g => new { Score = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var scoresDict = Enumerable.Range(1, 5)
            .ToDictionary(
                s => s,
                s => scoreCounts.FirstOrDefault(x => x.Score == s)?.Count ?? 0);

        var catCounts = await (
            from c in categories
            join e in entries on c.Id equals e.CategoryId into gj
            select new CategoryCount(c.Id, c.Name, gj.Count())
        ).ToListAsync(ct);

        // Round avg to 1 decimal to match example like 3.8
        var avgRounded = Math.Round(avg, 1, MidpointRounding.AwayFromZero);

        return new PulseSummaryReadModel(count, avgRounded, scoresDict, catCounts);
    }
}