using DuitTracker.Api.Shared.Domain;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DuitTracker.Api.Shared.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}

public class JwtService(IConfiguration config) : IJwtService
{
    public string GenerateToken(User user)
    {
        var secretKey = config["JwtSettings:SecretKey"]!;
        var issuer = config["JwtSettings:Issuer"]!;
        var audience = config["JwtSettings:Audience"]!;
        var expiryInMinutes = int.Parse(config["JwtSettings:ExpiryInMinutes"]!);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("fullName", user.FullName)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}