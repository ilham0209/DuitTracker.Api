using DuitTracker.Api.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Features.Categories;

public record GetAllCategoryQuery : IRequest<List<GetAllCategoryResponse>>;

public record GetAllCategoryResponse(Guid Id, string Name, string Icon, string Color, string Type);

public class GetAllCategoryHandler(DuitDbContext db) : IRequestHandler<GetAllCategoryQuery, List<GetAllCategoryResponse>>
{
    public async Task<List<GetAllCategoryResponse>> Handle(GetAllCategoryQuery request, CancellationToken ct)
    {
        return await db.Categories
            .Select(x => new GetAllCategoryResponse(x.Id, x.Name, x.Icon, x.Color, x.Type))
            .ToListAsync(ct);
    }
}