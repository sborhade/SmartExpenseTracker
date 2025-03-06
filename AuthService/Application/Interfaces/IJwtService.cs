using Domain.Entities;

namespace Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(LoginModel user);
}
