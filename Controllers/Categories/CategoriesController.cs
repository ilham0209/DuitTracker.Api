using DuitTracker.Api.Features.Categories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DuitTracker.Api.Controllers.Categories;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryCommand command, CancellationToken ct)
        => Ok(await sender.Send(command, ct));

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await sender.Send(new GetAllCategoryQuery(), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await sender.Send(new GetByIdCategoryQuery(id), ct));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, EditCategoryCommand command, CancellationToken ct)
        => Ok(await sender.Send(command with { Id = id }, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await sender.Send(new DeleteCategoryCommand(id), ct);
        return NoContent();
    }
}