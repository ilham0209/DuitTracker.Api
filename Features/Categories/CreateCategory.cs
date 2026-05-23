using DuitTracker.Api.Shared.Domain;
using DuitTracker.Api.Shared.Infrastructure.Persistence;
using DuitTracker.Api.Shared.Services;
using FluentValidation;
using MediatR;

namespace DuitTracker.Api.Features.Categories;

public record CreateCategoryCommand(string Name, string Icon, string Color, string Type) : IRequest<CreateCategoryResponse>;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Icon).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Color).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Type).NotEmpty().Must(t => t == "Income" || t == "Expense")
            .WithMessage("Type must be 'Income' or 'Expense'.");
    }
}

public record CreateCategoryResponse(Guid Id, string Name, string Icon, string Color, string Type);

public class CreateCategoryHandler(DuitDbContext db, ICurrentUserService currentUser) : IRequestHandler<CreateCategoryCommand, CreateCategoryResponse>
{
    public async Task<CreateCategoryResponse> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            UserId = currentUser.UserId,
            Name = request.Name,
            Icon = request.Icon,
            Color = request.Color,
            Type = request.Type
        };

        category.SetCreated(currentUser.Email);

        db.Categories.Add(category);
        await db.SaveChangesAsync(ct);

        return new CreateCategoryResponse(category.Id, category.Name, category.Icon, category.Color, category.Type);
    }
}