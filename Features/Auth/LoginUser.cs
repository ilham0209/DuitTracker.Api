using DuitTracker.Api.Shared.Infrastructure.Persistence;
using DuitTracker.Api.Shared.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Features.Auth;

public record LoginUserCommand(string Email, string Password) : IRequest<LoginUserResponse>;

public class LoginUserValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public record LoginUserResponse(Guid Id, string FullName, string Email, string Role, string Token);

public class LoginUserHandler(DuitDbContext db, IJwtService jwtService) : IRequestHandler<LoginUserCommand, LoginUserResponse>
{
    public async Task<LoginUserResponse> Handle(LoginUserCommand request, CancellationToken ct)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(x => x.Email == request.Email, ct)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        var hasher = new PasswordHasher<Shared.Domain.User>();
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var token = jwtService.GenerateToken(user);

        return new LoginUserResponse(user.Id, user.FullName, user.Email, user.Role, token);
    }
}