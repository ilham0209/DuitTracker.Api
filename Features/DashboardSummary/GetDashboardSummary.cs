using DuitTracker.Api.Shared.Infrastructure.Persistence;
using DuitTracker.Api.Shared.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Features.Dashboard;

public record GetDashboardSummaryQuery(int? Year) : IRequest<GetDashboardSummaryResponse>;

public record MonthlySummaryItem(
    int Month,
    int Year,
    decimal TotalIncome,
    decimal TotalExpense,
    decimal Balance,
    List<CategorySpendingItem> TopSpendingCategories);

public record CategorySpendingItem(
    string CategoryName,
    string CategoryIcon,
    string CategoryColor,
    decimal TotalSpent,
    decimal Percentage);

public record GetDashboardSummaryResponse(
    decimal TotalIncome,
    decimal TotalExpense,
    decimal CurrentBalance,
    List<MonthlySummaryItem> MonthlySummary);

public class GetDashboardSummaryHandler(DuitDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<GetDashboardSummaryQuery, GetDashboardSummaryResponse>
{
    public async Task<GetDashboardSummaryResponse> Handle(GetDashboardSummaryQuery request, CancellationToken ct)
    {
        var query = db.Transactions
            .Include(x => x.Category)
            .Where(x => x.UserId == currentUser.UserId);

        if (request.Year.HasValue)
            query = query.Where(x => x.TransactionDate.Year == request.Year.Value);

        var transactions = await query
            .Select(x => new
            {
                x.Amount,
                x.TransactionDate,
                CategoryName = x.Category.Name,
                CategoryIcon = x.Category.Icon,
                CategoryColor = x.Category.Color,
                CategoryType = x.Category.Type
            })
            .ToListAsync(ct);

        var totalIncome = transactions
            .Where(x => x.CategoryType == "Income")
            .Sum(x => x.Amount);

        var totalExpense = transactions
            .Where(x => x.CategoryType == "Expense")
            .Sum(x => x.Amount);

        var currentBalance = totalIncome - totalExpense;

        var monthlySummary = transactions
            .GroupBy(x => new { x.TransactionDate.Month, x.TransactionDate.Year })
            .OrderBy(g => g.Key.Year)
            .ThenBy(g => g.Key.Month)
            .Select(g =>
            {
                var monthlyIncome = g.Where(x => x.CategoryType == "Income").Sum(x => x.Amount);
                var monthlyExpense = g.Where(x => x.CategoryType == "Expense").Sum(x => x.Amount);
                var monthlyBalance = monthlyIncome - monthlyExpense;

                var expenseTransactions = g.Where(x => x.CategoryType == "Expense").ToList();
                var totalMonthlyExpense = expenseTransactions.Sum(x => x.Amount);

                var topCategories = expenseTransactions
                    .GroupBy(x => new { x.CategoryName, x.CategoryIcon, x.CategoryColor })
                    .Select(cg => new CategorySpendingItem(
                        cg.Key.CategoryName,
                        cg.Key.CategoryIcon,
                        cg.Key.CategoryColor,
                        cg.Sum(x => x.Amount),
                        totalMonthlyExpense == 0 ? 0 : Math.Round(cg.Sum(x => x.Amount) / totalMonthlyExpense * 100, 2)))
                    .OrderByDescending(x => x.TotalSpent)
                    .Take(5)
                    .ToList();

                return new MonthlySummaryItem(
                    g.Key.Month,
                    g.Key.Year,
                    monthlyIncome,
                    monthlyExpense,
                    monthlyBalance,
                    topCategories);
            })
            .ToList();

        return new GetDashboardSummaryResponse(totalIncome, totalExpense, currentBalance, monthlySummary);
    }
}