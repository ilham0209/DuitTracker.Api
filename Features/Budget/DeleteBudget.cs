using DuitTracker.Api.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Features.Budgets;

public record DeleteBudgetCommand(Guid Id) : IRequest;

public class DeleteBudgetHandler(DuitDbContext db) : IRequestHandler<DeleteBudgetCommand>
{
    public async Task Handle(DeleteBudgetCommand request, CancellationToken ct)
    {
        var budget = await db.Budgets
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Budget with ID {request.Id} not found.");

        budget.SetDeleted();
        budget.SetModified("system");

        await db.SaveChangesAsync(ct);
    }
}