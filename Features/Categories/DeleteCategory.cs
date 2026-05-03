using DuitTracker.Api.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Features.Categories;

public static class DeleteCategory
{
    public record Command(Guid Id) : IRequest<bool>;

    public class Handler(DuitDbContext context) : IRequestHandler<Command, bool>
    {
        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
        {
            var entity = await context.Categories.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (entity is null) return false;

            entity.SetDeleted();
            await context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}