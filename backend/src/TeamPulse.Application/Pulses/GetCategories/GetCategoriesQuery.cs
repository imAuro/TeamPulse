using MediatR;

namespace TeamPulse.Application.Pulses.GetCategories;

public sealed record GetCategoriesQuery : IRequest<IReadOnlyList<CategoryDto>>;

public sealed record CategoryDto(Guid Id, string Name);