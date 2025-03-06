using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task Register(RegisterModel model)
    {
        await _authService.RegisterAsync(model);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        return Ok(new { Token = await _authService.LoginAsync(model) });
    }
}
