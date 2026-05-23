using DuitTracker.Api.Shared.Infrastructure.Persistence;
using DuitTracker.Api.Shared.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Features.PaymentMethods;

public record GetAllPaymentMethodQuery : IRequest<List<GetAllPaymentMethodResponse>>;

public record GetAllPaymentMethodResponse(Guid Id, string Name);

public class GetAllPaymentMethodHandler(DuitDbContext db, ICurrentUserService currentUser) : IRequestHandler<GetAllPaymentMethodQuery, List<GetAllPaymentMethodResponse>>
{
    public async Task<List<GetAllPaymentMethodResponse>> Handle(GetAllPaymentMethodQuery request, CancellationToken ct)
    {
        return await db.PaymentMethods
            .Where(x => x.UserId == currentUser.UserId)
            .Select(x => new GetAllPaymentMethodResponse(x.Id, x.Name))
            .ToListAsync(ct);
    }
}