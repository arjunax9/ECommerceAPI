using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ECommerce.Business.Interfaces;
using ECommerce.Data.Repositories;
using ECommerce.Models.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ECommerce.Business.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<User> RegisterAsync(User user)
    {
        // Simple register: ensure email is unique
        var exists = await _userRepository.GetByEmailAsync(user.Email);
        if (exists != null)
            throw new InvalidOperationException("User with this email already exists.");
        // Hash the password before storing
        var plain = user.Password ?? string.Empty;
        user.Password = PasswordHasher.Hash(plain);

        var created = await _userRepository.AddAsync(user);

        return created;
    }

    public async Task<string?> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
            return null;

        // Verify password using hasher
        if (!PasswordHasher.Verify(user.Password ?? string.Empty, password))
            return null;

        var jwtKey = _configuration["Jwt:Key"] ?? string.Empty;
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? string.Empty;
        var jwtAudience = _configuration["Jwt:Audience"] ?? string.Empty;

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("name", user.Name ?? string.Empty)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(6),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
