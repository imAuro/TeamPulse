using FluentValidation;
using TeamPulse.Domain.Pulses;

namespace TeamPulse.Application.Pulses.SubmitPulse;

public sealed class SubmitPulseValidator : AbstractValidator<SubmitPulseCommand>
{
    public SubmitPulseValidator()
    {
        RuleFor(x => x.Score).InclusiveBetween(1, 5);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Comment)
            .MaximumLength(PulseEntry.MaxCommentLength)
            .When(x => x.Comment is not null);
    }
}