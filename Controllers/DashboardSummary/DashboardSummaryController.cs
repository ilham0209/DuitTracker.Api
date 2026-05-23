using DuitTracker.Api.Features.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DuitTracker.Api.Controllers.Dashboard;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetSummary([FromQuery] int? year, CancellationToken ct)
        => Ok(await sender.Send(new GetDashboardSummaryQuery(year), ct));
}