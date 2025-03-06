namespace Application.Interfaces;
public interface IAuthService
{
    Task RegisterAsync(RegisterModel registerModel);
    Task<string> LoginAsync(LoginModel loginModel);
    Task<bool> IsPasswordValidAsync(string password, string hashedPassword);
}
