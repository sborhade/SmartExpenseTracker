namespace Domain.Entities;
public class ApplicationUser
{
    public required string Id { get; set; }
    public string? Email { get; set; }
    public required string PasswordHash { get; set; }
}