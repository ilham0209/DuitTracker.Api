using DuitTracker.Api.Features.PaymentMethods;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DuitTracker.Api.Controllers.PaymentMethods;

[ApiController]
[Route("api/[controller]")]
public class PaymentMethodsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreatePaymentMethodCommand command, CancellationToken ct)
        => Ok(await sender.Send(command, ct));

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await sender.Send(new GetAllPaymentMethodQuery(), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await sender.Send(new GetByIdPaymentMethodQuery(id), ct));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, EditPaymentMethodCommand command, CancellationToken ct)
        => Ok(await sender.Send(command with { Id = id }, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await sender.Send(new DeletePaymentMethodCommand(id), ct);
        return NoContent();
    }
}