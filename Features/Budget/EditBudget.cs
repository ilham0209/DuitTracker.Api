using DuitTracker.Api.Shared.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Features.Budgets;

public record EditBudgetCommand(
    Guid Id,
    Guid CategoryId,
    decimal Amount,
    int Month,
    int Year) : IRequest<EditBudgetResponse>;

public class EditBudgetValidator : AbstractValidator<EditBudgetCommand>
{
    public EditBudgetValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Month).InclusiveBetween(1, 12);
        RuleFor(x => x.Year).GreaterThan(0);
    }
}

public record EditBudgetResponse(
    Guid Id,
    Guid CategoryId,
    string CategoryName,
    string CategoryType,
    decimal Amount,
    int Month,
    int Year);

public class EditBudgetHandler(DuitDbContext db) : IRequestHandler<EditBudgetCommand, EditBudgetResponse>
{
    public async Task<EditBudgetResponse> Handle(EditBudgetCommand request, CancellationToken ct)
    {
        var budget = await db.Budgets
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Budget with ID {request.Id} not found.");

        var category = await db.Categories
            .FirstOrDefaultAsync(x => x.Id == request.CategoryId, ct)
            ?? throw new KeyNotFoundException($"Category with ID {request.CategoryId} not found.");

        budget.CategoryId = request.CategoryId;
        budget.Amount = request.Amount;
        budget.Month = request.Month;
        budget.Year = request.Year;
        budget.SetModified("system");

        await db.SaveChangesAsync(ct);

        return new EditBudgetResponse(
            budget.Id,
            category.Id,
            category.Name,
            category.Type,
            budget.Amount,
            budget.Month,
            budget.Year);
    }
}