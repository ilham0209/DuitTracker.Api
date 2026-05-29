using DuitTracker.Api.Shared.Domain;
using DuitTracker.Api.Shared.Infrastructure.Persistence;
using DuitTracker.Api.Shared.Services;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DuitTracker.Api.Features.Auth;

public record ForgotPasswordCommand(string Email) : IRequest<ForgotPasswordResponse>;

public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

public record ForgotPasswordResponse(string Message);

public class ForgotPasswordHandler(DuitDbContext db, IEmailService emailService)
    : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponse>
{
    public async Task<ForgotPasswordResponse> Handle(ForgotPasswordCommand request, CancellationToken ct)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(x => x.Email == request.Email, ct);

        if (user != null)
        {
            var existingTokens = await db.PasswordResetTokens
                .Where(x => x.UserId == user.Id && !x.IsUsed)
                .ToListAsync(ct);

            db.PasswordResetTokens.RemoveRange(existingTokens);

            var token = new PasswordResetToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = Guid.NewGuid().ToString(),
                ExpiryDate = DateTime.UtcNow.AddMinutes(15),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            db.PasswordResetTokens.Add(token);
            await db.SaveChangesAsync(ct);

            var resetUrl = $"http://localhost:5000/reset-password?token={token.Token}";

            var emailBody = $"""
                <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
                    <h2 style="color: #333;">Reset Your Password</h2>
                    <p>Hi {user.FullName},</p>
                    <p>We received a request to reset your DuitTracker password.</p>
                    <p>Click the button below to reset your password. This link will expire in <strong>15 minutes</strong>.</p>
                    <a href="{resetUrl}" 
                       style="display: inline-block; padding: 12px 24px; background-color: #4CAF50; color: white; text-decoration: none; border-radius: 4px; margin: 16px 0;">
                        Reset Password
                    </a>
                    <p>If you did not request a password reset, please ignore this email.</p>
                    <p>This link can only be used once.</p>
                    <hr style="border: none; border-top: 1px solid #eee; margin: 24px 0;" />
                    <p style="color: #999; font-size: 12px;">DuitTracker — Personal Finance Tracker</p>
                </div>
                """;

            await emailService.SendEmailAsync(user.Email, "Reset Your DuitTracker Password", emailBody);
        }

        return new ForgotPasswordResponse("If the account exists, a reset link has been sent.");
    }
}