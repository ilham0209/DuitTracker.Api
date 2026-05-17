using DuitTracker.Api.Features.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DuitTracker.Api.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserCommand command, CancellationToken ct)
        => Ok(await sender.Send(command, ct));

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserCommand command, CancellationToken ct)
        => Ok(await sender.Send(command, ct));
}