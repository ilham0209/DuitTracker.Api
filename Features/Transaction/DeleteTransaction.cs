using DuitTracker.Api.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Features.Transactions;

public record DeleteTransactionCommand(Guid Id) : IRequest;

public class DeleteTransactionHandler(DuitDbContext db) : IRequestHandler<DeleteTransactionCommand>
{
    public async Task Handle(DeleteTransactionCommand request, CancellationToken ct)
    {
        var transaction = await db.Transactions
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Transaction with ID {request.Id} not found.");

        transaction.SetDeleted();
        transaction.SetModified("system");

        await db.SaveChangesAsync(ct);
    }
}