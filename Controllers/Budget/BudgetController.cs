using DuitTracker.Api.Features.Budgets;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DuitTracker.Api.Controllers.Budgets;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BudgetsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateBudgetCommand command, CancellationToken ct)
        => Ok(await sender.Send(command, ct));

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await sender.Send(new GetAllBudgetQuery(), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await sender.Send(new GetByIdBudgetQuery(id), ct));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, EditBudgetCommand command, CancellationToken ct)
        => Ok(await sender.Send(command with { Id = id }, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await sender.Send(new DeleteBudgetCommand(id), ct);
        return NoContent();
    }
}