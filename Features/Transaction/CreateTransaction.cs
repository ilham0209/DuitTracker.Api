using DuitTracker.Api.Shared.Domain;
using DuitTracker.Api.Shared.Infrastructure.Persistence;
using DuitTracker.Api.Shared.Services;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Features.Transactions;

public record CreateTransactionCommand(
    Guid CategoryId,
    Guid PaymentMethodId,
    decimal Amount,
    string Note,
    DateTime TransactionDate,
    string ReferenceNo,
    string AttachmentUrl) : IRequest<CreateTransactionResponse>;

public class CreateTransactionValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.PaymentMethodId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Note).MaximumLength(500);
        RuleFor(x => x.TransactionDate).NotEmpty();
        RuleFor(x => x.ReferenceNo).MaximumLength(100);
        RuleFor(x => x.AttachmentUrl).MaximumLength(500);
    }
}

public record CreateTransactionResponse(
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

public class CreateTransactionHandler(DuitDbContext db, ICurrentUserService currentUser) : IRequestHandler<CreateTransactionCommand, CreateTransactionResponse>
{
    public async Task<CreateTransactionResponse> Handle(CreateTransactionCommand request, CancellationToken ct)
    {
        var category = await db.Categories
            .FirstOrDefaultAsync(x => x.Id == request.CategoryId && x.UserId == currentUser.UserId, ct)
            ?? throw new KeyNotFoundException($"Category with ID {request.CategoryId} not found.");

        var paymentMethod = await db.PaymentMethods
            .FirstOrDefaultAsync(x => x.Id == request.PaymentMethodId && x.UserId == currentUser.UserId, ct)
            ?? throw new KeyNotFoundException($"Payment method with ID {request.PaymentMethodId} not found.");

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            UserId = currentUser.UserId,
            CategoryId = request.CategoryId,
            PaymentMethodId = request.PaymentMethodId,
            Amount = request.Amount,
            Note = request.Note,
            TransactionDate = request.TransactionDate.ToUniversalTime(),
            ReferenceNo = request.ReferenceNo,
            AttachmentUrl = request.AttachmentUrl
        };

        transaction.SetCreated(currentUser.Email);

        db.Transactions.Add(transaction);
        await db.SaveChangesAsync(ct);

        return new CreateTransactionResponse(
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