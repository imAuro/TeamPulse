using TeamPulse.Application.Pulses.GetPulseSummary;
using TeamPulse.Domain.Pulses;

namespace TeamPulse.Application.Common.Abstractions;

public interface IPulseRepository
{
    Task<bool> CategoryExistsAsync(Guid categoryId, CancellationToken ct);
    Task<IReadOnlyList<PulseCategory>> GetCategoriesAsync(CancellationToken ct);

    Task AddPulseAsync(PulseEntry entry, CancellationToken ct);

    Task<PulseSummaryReadModel> GetSummaryAsync(CancellationToken ct);
}