using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseService.Presentation;

[Authorize]
[Route("api/expenses")]
[ApiController]
public class ExpenseController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpenseController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExpenseDto>>> GetExpenses()
    {
        var expenses = await _expenseService.GetAllExpensesAsync();
        return Ok(expenses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExpenseDto>> GetExpense(int id)
    {
        var expense = await _expenseService.GetExpenseByIdAsync(id);
        if (expense == null)
        {
            return NotFound("Expense not found.");
        }
        return Ok(expense);
    }

    [HttpPost("add")]
    public async Task<ActionResult<ExpenseDto>> CreateExpense([FromBody] ExpenseDto expenseDto)
    {
        await _expenseService.AddExpenseAsync(expenseDto);
        return Ok("Expense added successfully.");
    }

    [HttpPut("update/{id}")]
    public async Task<ActionResult<ExpenseDto>> UpdateExpense(int id, [FromBody] ExpenseDto expenseDto)
    {
        await _expenseService.UpdateExpenseAsync(id, expenseDto);
        return Ok("Expense updated successfully.");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteExpense(int id)
    {
        await _expenseService.DeleteExpenseAsync(id);
        return NoContent();
    }
}