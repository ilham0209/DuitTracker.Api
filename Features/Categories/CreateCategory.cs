using DuitTracker.Api.Shared.Domain;
using DuitTracker.Api.Shared.Infrastructure.Persistence;
using FluentValidation;
using MediatR;

namespace DuitTracker.Api.Features.Categories;

public static class CreateCategory
{
    public record Command(string Name, string Icon, string Type) : IRequest<Response>;
    public record Response(Guid Id, string Name, string Icon, string Type);

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100).WithMessage("Name is required");
            RuleFor(x => x.Icon).NotEmpty().WithMessage("Icon is required");
            RuleFor(x => x.Type).NotEmpty().Must(x => x == "Income" || x == "Expense").WithMessage("Type must be Income or Expense");
        }
    }

    public class Handler(DuitDbContext context, IHttpContextAccessor httpContextAccessor) : IRequestHandler<Command, Response>
    {
        private readonly IValidator<Command> _validator = new Validator();

        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var user = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

            var entity = new Category
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Icon = request.Icon,
                Type = request.Type
            };

            entity.SetCreated(user);
            context.Categories.Add(entity);
            await context.SaveChangesAsync(cancellationToken);

            return new Response(entity.Id, entity.Name, entity.Icon, entity.Type);
        }
    }
}