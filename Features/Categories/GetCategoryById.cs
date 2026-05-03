using DuitTracker.Api.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Features.Categories;

public static class GetCategoryById
{
    public record Query(Guid Id) : IRequest<Response?>;
    public record Response(Guid Id, string Name, string Icon, string Type);

    public class Handler(DuitDbContext context) : IRequestHandler<Query, Response?>
    {
        public async Task<Response?> Handle(Query request, CancellationToken cancellationToken)
        {
            return await context.Categories
                .Where(x => x.Id == request.Id)
                .Select(x => new Response(x.Id, x.Name, x.Icon, x.Type))
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}