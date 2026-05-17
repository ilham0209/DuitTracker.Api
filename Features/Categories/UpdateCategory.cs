using DuitTracker.Api.Shared.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Features.Categories;

public record EditCategoryCommand(Guid Id, string Name, string Icon, string Color, string Type) : IRequest<EditCategoryResponse>;

public class EditCategoryValidator : AbstractValidator<EditCategoryCommand>
{
    public EditCategoryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Icon).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Color).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Type).NotEmpty().Must(t => t == "Income" || t == "Expense")
            .WithMessage("Type must be 'Income' or 'Expense'.");
    }
}

public record EditCategoryResponse(Guid Id, string Name, string Icon, string Color, string Type);

public class EditCategoryHandler(DuitDbContext db) : IRequestHandler<EditCategoryCommand, EditCategoryResponse>
{
    public async Task<EditCategoryResponse> Handle(EditCategoryCommand request, CancellationToken ct)
    {
        var category = await db.Categories
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Category with ID {request.Id} not found.");

        category.Name = request.Name;
        category.Icon = request.Icon;
        category.Color = request.Color;
        category.Type = request.Type;
        category.SetModified("system");

        await db.SaveChangesAsync(ct);

        return new EditCategoryResponse(category.Id, category.Name, category.Icon, category.Color, category.Type);
    }
}