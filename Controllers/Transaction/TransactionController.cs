using DuitTracker.Api.Features.Transactions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DuitTracker.Api.Controllers.Transactions;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateTransactionCommand command, CancellationToken ct)
        => Ok(await sender.Send(command, ct));

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await sender.Send(new GetAllTransactionQuery(), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await sender.Send(new GetByIdTransactionQuery(id), ct));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, EditTransactionCommand command, CancellationToken ct)
        => Ok(await sender.Send(command with { Id = id }, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await sender.Send(new DeleteTransactionCommand(id), ct);
        return NoContent();
    }
}