using DuitTracker.Api.Shared.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Features.PaymentMethods;

public record EditPaymentMethodCommand(Guid Id, string Name) : IRequest<EditPaymentMethodResponse>;

public class EditPaymentMethodValidator : AbstractValidator<EditPaymentMethodCommand>
{
    public EditPaymentMethodValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

public record EditPaymentMethodResponse(Guid Id, string Name);

public class EditPaymentMethodHandler(DuitDbContext db) : IRequestHandler<EditPaymentMethodCommand, EditPaymentMethodResponse>
{
    public async Task<EditPaymentMethodResponse> Handle(EditPaymentMethodCommand request, CancellationToken ct)
    {
        var paymentMethod = await db.PaymentMethods
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Payment method with ID {request.Id} not found.");

        paymentMethod.Name = request.Name;
        paymentMethod.SetModified("system");

        await db.SaveChangesAsync(ct);

        return new EditPaymentMethodResponse(paymentMethod.Id, paymentMethod.Name);
    }
}