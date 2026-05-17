using DuitTracker.Api.Shared.Domain;
using DuitTracker.Api.Shared.Infrastructure.Persistence;
using FluentValidation;
using MediatR;

namespace DuitTracker.Api.Features.PaymentMethods;

public record CreatePaymentMethodCommand(string Name) : IRequest<CreatePaymentMethodResponse>;

public class CreatePaymentMethodValidator : AbstractValidator<CreatePaymentMethodCommand>
{
    public CreatePaymentMethodValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

public record CreatePaymentMethodResponse(Guid Id, string Name);

public class CreatePaymentMethodHandler(DuitDbContext db) : IRequestHandler<CreatePaymentMethodCommand, CreatePaymentMethodResponse>
{
    public async Task<CreatePaymentMethodResponse> Handle(CreatePaymentMethodCommand request, CancellationToken ct)
    {
        var paymentMethod = new PaymentMethod
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Empty,
            Name = request.Name
        };

        paymentMethod.SetCreated("system");

        db.PaymentMethods.Add(paymentMethod);
        await db.SaveChangesAsync(ct);

        return new CreatePaymentMethodResponse(paymentMethod.Id, paymentMethod.Name);
    }
}