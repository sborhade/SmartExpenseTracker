using System.ComponentModel;
using System.Text;
using System.Text.Json;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;

public class AIInsightsService : IAIinsightsService
{
    private readonly AIInsightsDbContext _dbContext;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    public AIInsightsService(AIInsightsDbContext dbContext, HttpClient httpClient, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task ProcessExpenseAsync(Expense expense)
    {
        //var category = await PredictCategoryAsync(expense.Description);
        //var sentiment = await AnalyzeSentimentAsync(expense.Description);
        var forecast = await ForecastExpensesAsync(expense.Amount);

        //Console.WriteLine($"description = {expense.Description}");
        //Console.WriteLine($"sentiment = {sentiment}");
        Console.WriteLine($"forecast = {forecast}");

        var insight = new AIInsight
        {
            ExpenseId = expense.Id,
            UserId = expense.UserId,
            Category = "category",
            //    Sentiment = sentiment,
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
            var requestBody = new
            {
                inputs = description,
                parameters = new { candidate_labels = candidateLabels }
            };

            // Add Authorization header with Bearer token
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuration["HUGGING_FACE_API_KEY"]}");


            // Call the Hugging Face API for zero-shot classification
            var response = await _httpClient.PostAsync("https://api-inference.huggingface.co/models/facebook/bart-large-mnli",
                new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json"));

            // Check for a successful response
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error in API request: {response.StatusCode}");
                return "Other";
            }

            var responseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response: {responseString}");

            // Deserialize the response into JSON
            var responseData = JsonSerializer.Deserialize<JsonElement>(responseString);

            // Extract the predicted label (category)
            var category = responseData.GetProperty("labels")[0].GetString() ?? string.Empty;
            Console.WriteLine($"Predicted category: {category}");

            return category;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error predicting category: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            return "Other";
        }
    }

    public async Task<string> AnalyzeSentimentAsync(string description)
    {
        try
        {
            // Add Authorization header with Bearer token
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuration["HUGGING_FACE_API_KEY"]}");

            // Prepare request payload
            var requestBody = new { inputs = description }; // Correct key for text input

            var response = await _httpClient.PostAsync(
                "https://api-inference.huggingface.co/models/distilbert-base-uncased-finetuned-sst-2-english",
                new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            );

            var responseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"API Response: {responseString}"); // Debugging

            // Deserialize response as a nested array
            var responseData = JsonSerializer.Deserialize<JsonElement>(responseString);

            // Ensure response is a nested array
            if (responseData.ValueKind == JsonValueKind.Array && responseData.GetArrayLength() > 0)
            {
                var innerArray = responseData[0]; // Get the inner array (first element)

                if (innerArray.ValueKind == JsonValueKind.Array && innerArray.GetArrayLength() > 0)
                {
                    var bestMatch = innerArray.EnumerateArray()
                        .OrderByDescending(item => item.GetProperty("score").GetDouble()) // Sort by highest score
                        .First();

                    var sentiment = bestMatch.GetProperty("label").GetString() ?? "Unknown";
                    return sentiment;
                }
            }

            return "Unknown"; // Fallback in case response is empty or unexpected
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error analyzing sentiment: {ex.Message}");
            return "Error";
        }
    }

    public async Task<string> ForecastExpensesAsync(decimal amount)
    {
        try
        {
            // Add Authorization header with Bearer token
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuration["HUGGING_FACE_API_KEY"]}");

            // Define the input for the model
            var requestBody = new
            {
                inputs = $"{amount}"
            };

            var apiUrl = "https://huggingface.co/spaces/sborhade/expense_model/predict";

            // Hugging Face API URL (use a model like "bigscience/bloom" or custom)
            var response = await _httpClient.PostAsync(
                apiUrl,
                new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            );

            // Check for successful response
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response: {responseString}"); // Debugging

            // Deserialize response
            var responseData = JsonSerializer.Deserialize<JsonElement>(responseString);

            // Extract generated text
            if (responseData.ValueKind == JsonValueKind.Array && responseData.GetArrayLength() > 0)
            {
                var generatedText = responseData[0].GetProperty("generated_text").GetString();
                return generatedText ?? "Unable to generate forecast.";
            }

            return "Unexpected response format.";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error forecasting expenses: {ex.Message}");
            return $"Unable to forecast expenses. Error: {ex.Message}";
        }
    }
}
