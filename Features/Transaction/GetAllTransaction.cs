using DuitTracker.Api.Shared.Infrastructure.Persistence;
using DuitTracker.Api.Shared.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Features.Transactions;

public record GetAllTransactionQuery : IRequest<List<GetAllTransactionResponse>>;

public record GetAllTransactionResponse(
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

public class GetAllTransactionHandler(DuitDbContext db, ICurrentUserService currentUser) : IRequestHandler<GetAllTransactionQuery, List<GetAllTransactionResponse>>
{
    public async Task<List<GetAllTransactionResponse>> Handle(GetAllTransactionQuery request, CancellationToken ct)
    {
        return await db.Transactions
            .Where(x => x.UserId == currentUser.UserId)
            .Include(x => x.Category)
            .Include(x => x.PaymentMethod)
            .Select(x => new GetAllTransactionResponse(
                x.Id,
                x.CategoryId,
                x.Category.Name,
                x.Category.Type,
                x.PaymentMethodId,
                x.PaymentMethod.Name,
                x.Amount,
                x.Note,
                x.TransactionDate,
                x.ReferenceNo,
                x.AttachmentUrl))
            .ToListAsync(ct);
    }
}