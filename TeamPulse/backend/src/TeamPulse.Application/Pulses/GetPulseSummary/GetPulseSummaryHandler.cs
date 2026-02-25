using MediatR;
using TeamPulse.Application.Common.Abstractions;

namespace TeamPulse.Application.Pulses.GetPulseSummary;

public sealed class GetPulseSummaryHandler(IPulseRepository repo)
    : IRequestHandler<GetPulseSummaryQuery, PulseSummaryReadModel>
{
    public Task<PulseSummaryReadModel> Handle(GetPulseSummaryQuery request, CancellationToken ct)
        => repo.GetSummaryAsync(ct);
}