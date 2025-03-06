using System.Text;
using System.Text.Json;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.SemanticKernel.Connectors.HuggingFace;
public class AIInsightsService : IAIinsightsService
{
    private readonly AIInsightsDbContext _dbContext;
    private readonly HttpClient _httpClient;
    public AIInsightsService(AIInsightsDbContext dbContext, HttpClient httpClient)
    {
        _dbContext = dbContext;
        _httpClient = httpClient;
    }

    public async Task ProcessExpenseAsync(Expense expense)
    {
        Console.WriteLine("Processing expense analysis...");
        var category = await PredictCategoryAsync(expense.Description);
        //        var sentiment = await AnalyzeSentimentAsync(expense.Description);
        //      var forecast = await ForecastExpensesAsync(expense.Amount);

        var insight = new AIInsight
        {
            ExpenseId = expense.Id,
            UserId = expense.UserId,
            Category = category,
            Sentiment = "sentiment",
            Forecast = "forecast",
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.AIInsights.Add(insight);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<string> PredictCategoryAsync(string description)
    {
        try
        {
            Console.WriteLine("Predicting category and labels...");

            // Define candidate labels (categories)
            var candidateLabels = new[] { "Food", "Transport", "Shopping", "Rent", "Health", "Technology" };

            // Define the request body with the description and labels
            var requestBody = new { text = description, labels = candidateLabels };

            // Call the Hugging Face API for zero-shot classification
            var response = await _httpClient.PostAsync("https://api-inference.huggingface.co/models/facebook/bart-large-mnli",
                new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json"));
            var responseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"stringgggggggggggggggggggggggg : {responseString}");
            var responseData = JsonSerializer.Deserialize<JsonElement>(responseString);
            Console.WriteLine($"Responseeeeeeee: {responseData}");
            // Extract the predicted label (category)
            var category = responseData.GetProperty("labels")[0].GetString() ?? string.Empty;
            Console.WriteLine($"Predicted categoryyyyyyyyyyyyyy: {category}");

            // Optionally, extract additional labels or categories predicted by the model
            var label = responseData.GetProperty("labels")
                .EnumerateArray()
                .Select(label => label.GetString())
                .FirstOrDefault();
            Console.WriteLine($"Predicted labelllllllllllllll: {label}");
            return category;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errorrrrrrrrrrrrrrrrr predicting category: {ex.Message}");
            Console.WriteLine($"Errorrrrrrrrrrrrrrrrr predicting category: {ex.StackTrace}");
            // Log the exception (not shown here) and return a fallback category
            return "Other";
        }
    }

    public async Task<string> AnalyzeSentimentAsync(string description)
    {
        var response = await _httpClient.PostAsync("https://api-inference.huggingface.co/models/distilbert-base-uncased-finetuned-sst-2-english",
            new StringContent(JsonSerializer.Serialize(new { text = description }), Encoding.UTF8, "application/json"));

        var responseString = await response.Content.ReadAsStringAsync();
        var responseData = JsonSerializer.Deserialize<JsonElement>(responseString);
        return responseData.GetProperty("label").GetString() ?? string.Empty;
    }

    public async Task<string> ForecastExpensesAsync(decimal amount)
    {
        try
        {
            // Simulate calling an external service to forecast expenses
            var requestBody = new { amount };
            var response = await _httpClient.PostAsync("https://api.example.com/forecast",
                new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode(); // Throw if not a success code

            var responseString = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<JsonElement>(responseString);

            // Extract forecasted amount from response
            var forecastedAmount = responseData.GetProperty("forecastedAmount").GetDecimal();
            return $"Next month you might spend around {forecastedAmount:C} (forecasted based on historical data).";
        }
        catch (Exception ex)
        {
            // Log the exception (not shown here) and return a fallback message
            return $"Unable to forecast expenses at this time. Please try again later. Error: {ex.Message}";
        }
    }


    using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class SmartExpenseTracker
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public SmartExpenseTracker(string apiKey)
    {
        _apiKey = apiKey;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    }

    // Method for receipt scanning (conceptual)
    public async Task<Expense> ProcessReceiptAsync(string receiptImagePath)
    {
        // Implement OCR logic here to extract text from the receipt image
        string extractedText = await ExtractTextFromReceipt(receiptImagePath);

        // Use NLP model to categorize the expense
        string category = await PredictCategory(extractedText);

        // Create an Expense object
        var expense = new Expense
        {
            Description = extractedText,
            Category = category,
            Amount = ExtractAmount(extractedText), // Implement logic to extract amount
            Date = DateTime.Now // You might want to extract this from the receipt as well
        };

        return expense;
    }

    // Method for predicting category using Hugging Face
    private async Task<string> PredictCategory(string description)
    {
        var requestBody = new { inputs = description };
        string jsonString = JsonSerializer.Serialize(requestBody);

        var response = await _httpClient.PostAsync(
            "https://api-inference.huggingface.co/models/google/flan-t5-base",
            new StringContent(jsonString, Encoding.UTF8, "application/json")
        );

        if (response.IsSuccessStatusCode)
        {
            var resultJson = await response.Content.ReadAsStringAsync();
            return ExtractCategoryFromResult(resultJson); // Implement logic to parse the result
        }

        throw new Exception("Error predicting category: " + response.ReasonPhrase);
    }

    // Placeholder for extracting text from receipt image using OCR (implement with actual OCR library)
    private async Task<string> ExtractTextFromReceipt(string receiptImagePath)
    {
        // Implement OCR logic here (e.g., call an OCR service or library)
        return "Extracted text from receipt"; // Replace with actual extraction logic
    }

    // Placeholder for extracting amount from text (implement your own logic)
    private decimal ExtractAmount(string text)
    {
        // Implement logic to parse amount from the extracted text
        return 0m; // Replace with actual parsing logic
    }

    private string ExtractCategoryFromResult(string resultJson)
    {
        // Implement JSON parsing logic to extract the predicted category from the result
        return "Predicted Category"; // Replace with actual extraction logic
    }
}

// Usage example
public static async Task Main(string[] args)
{
    string apiKey = "YOUR_HUGGING_FACE_API_KEY";
    SmartExpenseTracker tracker = new SmartExpenseTracker(apiKey);

    string receiptImagePath = "path/to/receipt/image.jpg";

    Expense expense = await tracker.ProcessReceiptAsync(receiptImagePath);

    Console.WriteLine($"Processed Expense: {expense.Description}, Category: {expense.Category}, Amount: {expense.Amount}");
}





}
