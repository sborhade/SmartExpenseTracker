using Domain.Entities;
namespace Application.Interfaces;
public interface IExpenseRepository
{
    Task<List<Expense>> GetAllExpensesAsync();
    Task<Expense> GetExpenseByIdAsync(int id);
    Task AddExpenseAsync(Expense expense);
    Task UpdateExpenseAsync(Expense expense);
    Task DeleteExpenseAsync(int id);
}
