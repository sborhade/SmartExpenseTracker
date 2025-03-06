// AIInsightsDbContext.cs
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;
public class AIInsightsDbContext : DbContext
{
    public AIInsightsDbContext(DbContextOptions<AIInsightsDbContext> options)
        : base(options) { }

    public DbSet<AIInsight> AIInsights { get; set; }
}

