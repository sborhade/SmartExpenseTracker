namespace Application.DTOs;
public class ExpenseDto
{
    public string? UserId { get; set; }
    public string? Category { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}
