using DuitTracker.Api.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Features.Transactions;

public record GetByIdTransactionQuery(Guid Id) : IRequest<GetByIdTransactionResponse>;

public record GetByIdTransactionResponse(
    Guid Id,
    Guid CategoryId,
    string CategoryName,
    string CategoryType,
    Guid PaymentMethodId,
    string PaymentMethodName,
    decimal Amount,
    string Note,
    DateTime TransactionDate,
    string ReferenceNo,
    string AttachmentUrl);

public class GetByIdTransactionHandler(DuitDbContext db) : IRequestHandler<GetByIdTransactionQuery, GetByIdTransactionResponse>
{
    public async Task<GetByIdTransactionResponse> Handle(GetByIdTransactionQuery request, CancellationToken ct)
    {
        var transaction = await db.Transactions
            .Include(x => x.Category)
            .Include(x => x.PaymentMethod)
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Transaction with ID {request.Id} not found.");

        return new GetByIdTransactionResponse(
            transaction.Id,
            transaction.CategoryId,
            transaction.Category.Name,
            transaction.Category.Type,
            transaction.PaymentMethodId,
            transaction.PaymentMethod.Name,
            transaction.Amount,
            transaction.Note,
            transaction.TransactionDate,
            transaction.ReferenceNo,
            transaction.AttachmentUrl);
    }
}