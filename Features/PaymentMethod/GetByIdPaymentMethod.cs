using DuitTracker.Api.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Features.PaymentMethods;

public record GetByIdPaymentMethodQuery(Guid Id) : IRequest<GetByIdPaymentMethodResponse>;

public record GetByIdPaymentMethodResponse(Guid Id, string Name);

public class GetByIdPaymentMethodHandler(DuitDbContext db) : IRequestHandler<GetByIdPaymentMethodQuery, GetByIdPaymentMethodResponse>
{
    public async Task<GetByIdPaymentMethodResponse> Handle(GetByIdPaymentMethodQuery request, CancellationToken ct)
    {
        var paymentMethod = await db.PaymentMethods
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
            ?? throw new KeyNotFoundException($"Payment method with ID {request.Id} not found.");

        return new GetByIdPaymentMethodResponse(paymentMethod.Id, paymentMethod.Name);
    }
}