using DuitTracker.Api.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Features.Categories;

public static class GetAllCategories
{
    public record Query : IRequest<List<Response>>;
    public record Response(Guid Id, string Name, string Icon, string Type);

    public class Handler(DuitDbContext context) : IRequestHandler<Query, List<Response>>
    {
        public async Task<List<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await context.Categories
                .Select(x => new Response(x.Id, x.Name, x.Icon, x.Type))
                .ToListAsync(cancellationToken);
        }
    }
}