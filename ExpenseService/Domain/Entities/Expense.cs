using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;
public class Expense
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string? Category { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}
