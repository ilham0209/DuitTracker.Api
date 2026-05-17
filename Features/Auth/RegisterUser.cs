using DuitTracker.Api.Shared.Domain;
using DuitTracker.Api.Shared.Infrastructure.Persistence;
using DuitTracker.Api.Shared.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Features.Auth;

public record RegisterUserCommand(
    string FullName,
    string Email,
    string Password) : IRequest<RegisterUserResponse>;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(150);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}

public record RegisterUserResponse(Guid Id, string FullName, string Email, string Role, string Token);

public class RegisterUserHandler(DuitDbContext db, IJwtService jwtService) : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
{
    public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        var emailExists = await db.Users.AnyAsync(x => x.Email == request.Email, ct);

        if (emailExists)
            throw new InvalidOperationException("Email is already registered.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Email = request.Email,
            Role = "User"
        };

        var hasher = new PasswordHasher<User>();
        user.PasswordHash = hasher.HashPassword(user, request.Password);
        user.SetCreated(request.Email);

        db.Users.Add(user);
        await db.SaveChangesAsync(ct);

        var token = jwtService.GenerateToken(user);

        return new RegisterUserResponse(user.Id, user.FullName, user.Email, user.Role, token);
    }
}