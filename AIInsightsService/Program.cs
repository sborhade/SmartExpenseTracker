using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Add Database (SQL Server)
builder.Services.AddDbContext<AIInsightsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Add Services
builder.Services.AddHttpClient();
builder.Services.AddScoped<IAIinsightsService, AIInsightsService>();
builder.Services.AddScoped<IExpenseProcessor, AIInsightsService>();
builder.Services.AddHostedService<KafkaConsumerService>();

// 🔹 Add Controllers
builder.Services.AddControllers();

var app = builder.Build();

// 🔹 Enable API Endpoints
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AI Insights API v1"));

app.MapControllers();
app.Run();