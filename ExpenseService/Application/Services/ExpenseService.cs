namespace Application.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

public class ExpensesService : IExpenseService
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IKafkaProducerService _kafkaProducer;
 

    public ExpensesService(IExpenseRepository expenseRepository, IKafkaProducerService kafkaProducer)
    {
        _expenseRepository = expenseRepository;
        _kafkaProducer = kafkaProducer;
    }
    
    public async Task AddExpenseAsync(ExpenseDto expenseDto)
    {
        var expenseEntity = new Expense
        {
            Category = expenseDto.Category,
            Amount = expenseDto.Amount,
            Date = expenseDto.Date,
            UserId = expenseDto.UserId
        };

        await _expenseRepository.AddExpenseAsync(expenseEntity);
        // Publish the expense to Kafka
        await _kafkaProducer.PublishExpenseAsync(expenseEntity); 
    }

    public async Task DeleteExpenseAsync(int id)
    {
        var expense = _expenseRepository.GetExpenseByIdAsync(id);
        if (expense == null)
        {
            throw new Exception($"Expense with ID {id} not found.");
        }

        await _expenseRepository.DeleteExpenseAsync(id);
    }

    public async Task<List<ExpenseDto>> GetAllExpensesAsync()
    {
        var expenses = await _expenseRepository.GetAllExpensesAsync();

        return expenses.Select(e => new ExpenseDto
        {
            Category = e.Category,
            Amount = e.Amount,
            Date = e.Date,
            UserId = e.UserId
        })
        .ToList();

    }

    public async Task<ExpenseDto> GetExpenseByIdAsync(int id)
    {
        var expense = await _expenseRepository.GetExpenseByIdAsync(id);

        if (expense == null)
        {
            throw new Exception($"Expense with ID {id} not found.");
        }

        return new ExpenseDto
        {
            Category = expense.Category,
            Amount = expense.Amount,
            Date = expense.Date,
            UserId = expense.UserId
        };
    }

    public async Task UpdateExpenseAsync(int id, ExpenseDto expenseDto)
    {
        var existingExpense = await _expenseRepository.GetExpenseByIdAsync(id);

        if (existingExpense == null)
        {
            throw new Exception($"Expense with ID {id} not found.");
        }

        existingExpense.Category = expenseDto.Category;
        existingExpense.Amount = expenseDto.Amount;
        existingExpense.Date = expenseDto.Date;
        existingExpense.UserId = expenseDto.UserId;

        await _expenseRepository.UpdateExpenseAsync(existingExpense);
    }
}