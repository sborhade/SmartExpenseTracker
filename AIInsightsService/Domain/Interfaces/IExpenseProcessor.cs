using Domain.Entities;

namespace Domain.Interfaces;
public interface IExpenseProcessor
{
    Task ProcessExpenseAsync(Expense expense);
}