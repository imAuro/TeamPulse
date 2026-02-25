using MediatR;
using TeamPulse.Application.Common.Abstractions;

namespace TeamPulse.Application.Pulses.GetCategories;

public sealed class GetCategoriesHandler(IPulseRepository repo)
    : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    public async Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken ct)
    {
        var cats = await repo.GetCategoriesAsync(ct);
        return cats.Select(c => new CategoryDto(c.Id, c.Name)).ToList();
    }
}