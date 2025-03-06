using Domain.Entities;

namespace Application.Interfaces;
public interface IUserRepository
{
    Task<ApplicationUser?> GetByEmailAsync(string email);
    Task AddUserAsync(ApplicationUser user);
}
