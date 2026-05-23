using DuitTracker.Api.Shared.Infrastructure.Persistence;
using DuitTracker.Api.Shared.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Features.Budgets;

public record GetAllBudgetQuery : IRequest<List<GetAllBudgetResponse>>;

public record GetAllBudgetResponse(
    Guid Id,
    Guid CategoryId,
    string CategoryName,
    string CategoryType,
    decimal Amount,
    int Month,
    int Year);

public class GetAllBudgetHandler(DuitDbContext db, ICurrentUserService currentUser) : IRequestHandler<GetAllBudgetQuery, List<GetAllBudgetResponse>>
{
    public async Task<List<GetAllBudgetResponse>> Handle(GetAllBudgetQuery request, CancellationToken ct)
    {
        return await db.Budgets
            .Where(x => x.UserId == currentUser.UserId)
            .Include(x => x.Category)
            .Select(x => new GetAllBudgetResponse(
                x.Id,
                x.CategoryId,
                x.Category.Name,
                x.Category.Type,
                x.Amount,
                x.Month,
                x.Year))
            .ToListAsync(ct);
    }
}