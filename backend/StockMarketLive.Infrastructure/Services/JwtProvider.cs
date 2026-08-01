using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StockMarketLive.Application.Interfaces;
using StockMarketLive.Application.Settings;
using StockMarketLive.Domain.Entities;

namespace StockMarketLive.Infrastructure.Services;

public sealed class JwtProvider : IJwtProvider
{
    private readonly JwtSettings _jwtSettings;

    public JwtProvider(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
    }

    public string Generate(User user)
    {
        var secretKey = _jwtSettings.Key;
        var issuer = _jwtSettings.Issuer;
        var audience = _jwtSettings.Audience;

        if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
        {
            throw new InvalidOperationException("JWT Settings are missing in configuration.");
        }

        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(JwtRegisteredClaimNames.Email, user.Email)
            // Notice: No Role claim is added for security/freshness reasons.
        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            null,
            DateTime.UtcNow.AddHours(8),
            signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
