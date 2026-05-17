using DuitTracker.Api.Shared.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Features.Transactions;

public record EditTransactionCommand(
    Guid Id,
    Guid CategoryId,
    Guid PaymentMethodId,
    decimal Amount,
    string Note,
    DateTime TransactionDate,
    string ReferenceNo,
    string AttachmentUrl) : IRequest<EditTransactionResponse>;

public class EditTransactionValidator : AbstractValidator<EditTransactionCommand>
{
    public EditTransactionValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.PaymentMethodId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Note).MaximumLength(500);
        RuleFor(x => x.TransactionDate).NotEmpty();
        RuleFor(x => x.ReferenceNo).MaximumLength(100);
        RuleFor(x => x.AttachmentUrl).MaximumLength(500);
    }
}

public record EditTransactionResponse(
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

public class EditTransactionHandler(DuitDbContext db) : IRequestHandler<EditTransactionCommand, EditTransactionResponse>
{
    public async Task<EditTransactionResponse> Handle(EditTransactionCommand request, CancellationToken ct)
    {
        var transaction = await db.Transactions
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Transaction with ID {request.Id} not found.");

        var category = await db.Categories
            .FirstOrDefaultAsync(x => x.Id == request.CategoryId, ct)
            ?? throw new KeyNotFoundException($"Category with ID {request.CategoryId} not found.");

        var paymentMethod = await db.PaymentMethods
            .FirstOrDefaultAsync(x => x.Id == request.PaymentMethodId, ct)
            ?? throw new KeyNotFoundException($"Payment method with ID {request.PaymentMethodId} not found.");

        transaction.CategoryId = request.CategoryId;
        transaction.PaymentMethodId = request.PaymentMethodId;
        transaction.Amount = request.Amount;
        transaction.Note = request.Note;
        transaction.TransactionDate = request.TransactionDate.ToUniversalTime();
        transaction.ReferenceNo = request.ReferenceNo;
        transaction.AttachmentUrl = request.AttachmentUrl;
        transaction.SetModified("system");

        await db.SaveChangesAsync(ct);

        return new EditTransactionResponse(
            transaction.Id,
            category.Id,
            category.Name,
            category.Type,
            paymentMethod.Id,
            paymentMethod.Name,
            transaction.Amount,
            transaction.Note,
            transaction.TransactionDate,
            transaction.ReferenceNo,
            transaction.AttachmentUrl);
    }
}