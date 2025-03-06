using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly ExpenseDbContext _context;

    public ExpenseRepository(ExpenseDbContext context)
    {
        _context = context;
    }

    public async Task AddExpenseAsync(Expense expense)
    {
        await _context.Expenses.AddAsync(expense);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteExpenseAsync(int id)
    {
        var expense = await _context.Expenses.FindAsync(id);
        if (expense == null || expense == default)
        {
            throw new Exception($"Expense with ID {id} not found.");
        }
        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Expense>> GetAllExpensesAsync()
    {
        return await _context.Expenses.ToListAsync();
    }

    public async Task<Expense> GetExpenseByIdAsync(int id)
    {
        var expense = await _context.Expenses.FindAsync(id);

        if (expense == null)
        {
            throw new Exception($"Expense with ID {id} not found.");
        }

        return expense;
    }
    public async Task UpdateExpenseAsync(Expense expense)
    {
        _context.Expenses.Update(expense);
        await _context.SaveChangesAsync();
    }
}