namespace Domain.Entities;
public class AIInsight
{
    public int ExpenseId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Sentiment { get; set; } = string.Empty;
    public string Forecast { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

