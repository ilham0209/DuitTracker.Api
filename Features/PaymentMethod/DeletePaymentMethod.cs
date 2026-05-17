using DuitTracker.Api.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Features.PaymentMethods;

public record DeletePaymentMethodCommand(Guid Id) : IRequest;

public class DeletePaymentMethodHandler(DuitDbContext db) : IRequestHandler<DeletePaymentMethodCommand>
{
    public async Task Handle(DeletePaymentMethodCommand request, CancellationToken ct)
    {
        var paymentMethod = await db.PaymentMethods
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Payment method with ID {request.Id} not found.");

        paymentMethod.SetDeleted();
        paymentMethod.SetModified("system");

        await db.SaveChangesAsync(ct);
    }
}