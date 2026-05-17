using DuitTracker.Api.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Features.Categories;

public record GetByIdCategoryQuery(Guid Id) : IRequest<GetByIdCategoryResponse>;

public record GetByIdCategoryResponse(Guid Id, string Name, string Icon, string Color, string Type);

public class GetByIdCategoryHandler(DuitDbContext db) : IRequestHandler<GetByIdCategoryQuery, GetByIdCategoryResponse>
{
    public async Task<GetByIdCategoryResponse> Handle(GetByIdCategoryQuery request, CancellationToken ct)
    {
        var category = await db.Categories
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Category with ID {request.Id} not found.");

        return new GetByIdCategoryResponse(category.Id, category.Name, category.Icon, category.Color, category.Type);
    }
}