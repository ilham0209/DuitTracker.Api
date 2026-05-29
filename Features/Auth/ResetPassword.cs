using DuitTracker.Api.Shared.Infrastructure.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Features.Auth;

public record ResetPasswordCommand(string Token, string NewPassword) : IRequest<ResetPasswordResponse>;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8);
    }
}

public record ResetPasswordResponse(string Message);

public class ResetPasswordHandler(DuitDbContext db) : IRequestHandler<ResetPasswordCommand, ResetPasswordResponse>
{
    public async Task<ResetPasswordResponse> Handle(ResetPasswordCommand request, CancellationToken ct)
    {
        var resetToken = await db.PasswordResetTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == request.Token, ct)
            ?? throw new KeyNotFoundException("Invalid or expired reset token.");

        if (resetToken.IsUsed)
            throw new InvalidOperationException("This reset link has already been used.");

        if (resetToken.ExpiryDate < DateTime.UtcNow)
            throw new InvalidOperationException("This reset link has expired. Please request a new one.");

        var hasher = new PasswordHasher<Shared.Domain.User>();
        resetToken.User.PasswordHash = hasher.HashPassword(resetToken.User, request.NewPassword);

        resetToken.IsUsed = true;

        await db.SaveChangesAsync(ct);

        return new ResetPasswordResponse("Password has been reset successfully.");
    }
}