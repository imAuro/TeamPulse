using MediatR;
using TeamPulse.Application.Common.Abstractions;
using TeamPulse.Domain.Pulses;

namespace TeamPulse.Application.Pulses.SubmitPulse;

public sealed class SubmitPulseHandler(IPulseRepository repo) : IRequestHandler<SubmitPulseCommand>
{
    public async Task Handle(SubmitPulseCommand request, CancellationToken ct)
    {
        if (!await repo.CategoryExistsAsync(request.CategoryId, ct))
            throw new ArgumentException("CategoryId does not exist.");

        var entry = PulseEntry.Create(request.Score, request.Comment, request.CategoryId, DateTimeOffset.UtcNow);

        await repo.AddPulseAsync(entry, ct);
    }
}