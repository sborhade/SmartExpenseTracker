using Domain.Entities;
namespace Domain.Interfaces;
public interface IAIinsightsService : IExpenseProcessor
{
    Task<string> AnalyzeSentimentAsync(string description);
    Task<string> ForecastExpensesAsync(decimal amount);

    Task<string> PredictCategoryAsync(string description);
}