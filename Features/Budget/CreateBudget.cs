using DuitTracker.Api.Shared.Domain;
using DuitTracker.Api.Shared.Infrastructure.Persistence;
using DuitTracker.Api.Shared.Services;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Features.Budgets;

public record CreateBudgetCommand(
    Guid CategoryId,
    decimal Amount,
    int Month,
    int Year) : IRequest<CreateBudgetResponse>;

public class CreateBudgetValidator : AbstractValidator<CreateBudgetCommand>
{
    public CreateBudgetValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Month).InclusiveBetween(1, 12);
        RuleFor(x => x.Year).GreaterThan(0);
    }
}

public record CreateBudgetResponse(
    Guid Id,
    Guid CategoryId,
    string CategoryName,
    string CategoryType,
    decimal Amount,
    int Month,
    int Year);

public class CreateBudgetHandler(DuitDbContext db, ICurrentUserService currentUser) : IRequestHandler<CreateBudgetCommand, CreateBudgetResponse>
{
    public async Task<CreateBudgetResponse> Handle(CreateBudgetCommand request, CancellationToken ct)
    {
        var category = await db.Categories
            .FirstOrDefaultAsync(x => x.Id == request.CategoryId && x.UserId == currentUser.UserId, ct)
            ?? throw new KeyNotFoundException($"Category with ID {request.CategoryId} not found.");

        var budget = new Budget
        {
            Id = Guid.NewGuid(),
            UserId = currentUser.UserId,
            CategoryId = request.CategoryId,
            Amount = request.Amount,
            Month = request.Month,
            Year = request.Year
        };

        budget.SetCreated(currentUser.Email);

        db.Budgets.Add(budget);
        await db.SaveChangesAsync(ct);

        return new CreateBudgetResponse(
            budget.Id,
            category.Id,
            category.Name,
            category.Type,
            budget.Amount,
            budget.Month,
            budget.Year);
    }
}