using Application.DTOs;

namespace Application.Interfaces
{
    public interface IExpenseService
    {
        Task<List<ExpenseDto>> GetAllExpensesAsync();
        Task<ExpenseDto> GetExpenseByIdAsync(int id);
        Task AddExpenseAsync(ExpenseDto expense);
        Task UpdateExpenseAsync(int id, ExpenseDto expense);
        Task DeleteExpenseAsync(int id);
    }
}
