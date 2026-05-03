using DuitTracker.Api.Shared.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Features.Categories;

public static class UpdateCategory
{
    public record Command(Guid Id, string Name, string Icon, string Type) : IRequest<Response?>;
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

    public class Handler(DuitDbContext context, IHttpContextAccessor httpContextAccessor) : IRequestHandler<Command, Response?>
    {
        private readonly IValidator<Command> _validator = new Validator();

        public async Task<Response?> Handle(Command request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var entity = await context.Categories.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (entity is null) return null;

            var user = httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

            entity.Name = request.Name;
            entity.Icon = request.Icon;
            entity.Type = request.Type;
            entity.SetModified(user);

            await context.SaveChangesAsync(cancellationToken);

            return new Response(entity.Id, entity.Name, entity.Icon, entity.Type);
        }
    }
}