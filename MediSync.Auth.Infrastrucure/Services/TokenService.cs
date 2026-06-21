using MediSync.Auth.Application.Abstractions;
using MediSync.Auth.Domain.Aggregates;
using MediSync.Auth.Domain.Constants;
using MediSync.Auth.Domain.Utilities.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MediSync.Auth.Infrastrucure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        // Claims are pieces of information about the user that are encoded in the token. You can add more claims as needed.
        var claims = new List<Claim>
        {
            new (MediSyncClaimTypes.UserID, user.Id.ToString()),
            new (MediSyncClaimTypes.Role, user.Role.ToString()),
            new (JwtRegisteredClaimNames.Email, user.Email),
            new (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        if (user.Role == UserRole.Patient)
        {
            claims.Add(new Claim(MediSyncClaimTypes.PatientId, user.Id.ToString()));
        }

        if (user.Role == UserRole.Doctor)
        {
            claims.Add(new Claim(MediSyncClaimTypes.DoctorId, user.Id.ToString()));
        }

        var secret = _configuration["JwtSettings:SecretKey"]?.ToString();
        var issuer = _configuration["JwtSettings:Issuer"]?.ToString();
        var audience = _configuration["JwtSettings:Audience"]?.ToString();
        var expiryMinutes = int.Parse(_configuration["JwtSettings:ExpiryMinutes"] ?? "60");

        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!));
        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
