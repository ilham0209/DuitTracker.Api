using DuitTracker.Api.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Features.Categories;

public record DeleteCategoryCommand(Guid Id) : IRequest;

public class DeleteCategoryHandler(DuitDbContext db) : IRequestHandler<DeleteCategoryCommand>
{
    public async Task Handle(DeleteCategoryCommand request, CancellationToken ct)
    {
        var category = await db.Categories
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Category with ID {request.Id} not found.");

        category.SetDeleted();
        category.SetModified("system");

        await db.SaveChangesAsync(ct);
    }
}