using DuitTracker.Api.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Features.Budgets;

public record GetByIdBudgetQuery(Guid Id) : IRequest<GetByIdBudgetResponse>;

public record GetByIdBudgetResponse(
    Guid Id,
    Guid CategoryId,
    string CategoryName,
    string CategoryType,
    decimal Amount,
    int Month,
    int Year);

public class GetByIdBudgetHandler(DuitDbContext db) : IRequestHandler<GetByIdBudgetQuery, GetByIdBudgetResponse>
{
    public async Task<GetByIdBudgetResponse> Handle(GetByIdBudgetQuery request, CancellationToken ct)
    {
        var budget = await db.Budgets
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Budget with ID {request.Id} not found.");

        return new GetByIdBudgetResponse(
            budget.Id,
            budget.CategoryId,
            budget.Category.Name,
            budget.Category.Type,
            budget.Amount,
            budget.Month,
            budget.Year);
    }
}