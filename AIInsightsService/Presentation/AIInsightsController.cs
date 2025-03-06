using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/insights")]
[ApiController]
public class AIInsightsController : ControllerBase
{
    private readonly AIInsightsDbContext _dbContext;

    public AIInsightsController(AIInsightsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserInsights(int userId)
    {
        var insights = await _dbContext.AIInsights
            .Where(i => i.UserId == userId.ToString())
            .ToListAsync();

        return Ok(insights);
    }
}
