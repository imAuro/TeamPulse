using MediatR;

namespace TeamPulse.Application.Pulses.GetPulseSummary;

public sealed record GetPulseSummaryQuery : IRequest<PulseSummaryReadModel>;