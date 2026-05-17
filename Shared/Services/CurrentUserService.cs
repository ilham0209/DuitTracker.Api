using System.Security.Claims;

namespace DuitTracker.Api.Shared.Services;

public interface ICurrentUserService
{
    Guid UserId { get; }
    string Email { get; }
    string Role { get; }
}

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid UserId =>
        Guid.Parse(httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? httpContextAccessor.HttpContext.User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException("User is not authenticated."));

    public string Email =>
        httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email)
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

    public string Role =>
        httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Role)
            ?? throw new UnauthorizedAccessException("User is not authenticated.");
}