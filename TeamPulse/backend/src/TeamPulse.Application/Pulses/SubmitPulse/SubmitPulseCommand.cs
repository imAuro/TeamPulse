using MediatR;

namespace TeamPulse.Application.Pulses.SubmitPulse;


public sealed record SubmitPulseCommand(int Score, string? Comment, Guid CategoryId) : IRequest;