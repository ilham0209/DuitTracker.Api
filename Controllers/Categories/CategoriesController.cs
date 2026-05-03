using DuitTracker.Api.Features.Categories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DuitTracker.Api.Controllers.Categories;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CreateCategory.Response>> Create(CreateCategory.Command command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<List<GetAllCategories.Response>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetAllCategories.Query(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetCategoryById.Response>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCategoryById.Query(id), cancellationToken);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UpdateCategory.Response>> Update(Guid id, UpdateCategory.Command command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command with { Id = id }, cancellationToken);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteCategory.Command(id), cancellationToken);
        if (!result) return NotFound();
        return NoContent();
    }
}