using System.Security.Claims;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;
namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    private readonly IConfiguration _configuration;

    public AuthService(IPasswordHasher passwordHasher, IUserRepository userRepository, IJwtService jwtService, IConfiguration configuration)
    {
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
        _jwtService = jwtService;
        _configuration = configuration;
    }

    public async Task RegisterAsync(RegisterModel registerModel)
    {
        // Check if user already exists
        var existingUser = await _userRepository.GetByEmailAsync(registerModel.Email);
        if (existingUser != null)
        {
            throw new Exception("User already exists.");
        }

        // Hash the password
        var hashedPassword = _passwordHasher.HashPassword(registerModel.Password);

        // Save the new user
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = registerModel.Email,
            PasswordHash = hashedPassword
        };

        await _userRepository.AddUserAsync(user);
    }

    public async Task<string> LoginAsync(LoginModel loginModel)
    {
        var user = await _userRepository.GetByEmailAsync(loginModel.Email);
        if (user == null || !_passwordHasher.VerifyPassword(loginModel.Password, user.PasswordHash))
        {
            throw new Exception("Invalid credentials.");
        }

        return _jwtService.GenerateToken(loginModel);
    }

    public Task<bool> IsPasswordValidAsync(string password, string hashedPassword)
    {
        throw new NotImplementedException();
    }
}
