
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class JwtService : IJwtService
{
    private IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(LoginModel user)
    {
        var claims = new[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Email),
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Aud, user.ServiceName?.ToString() ?? string.Empty) // Set audience dynamically based on the service
        };

        if (_configuration["Jwt:Key"] == null)
        {
            throw new Exception("Secret key is not set in the configuration.");
        }

        var secretKey = _configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new Exception("Secret key is not set in the configuration.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: user.ServiceName?.ToString() ?? string.Empty, // Dynamic audience value
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15), // Adjust token expiration as needed
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}