using System.Text;
using System.Text.Json;
using ASGBackend.Models;

namespace ASGBackend.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-latest:generateContent";
        private readonly ILogger<GeminiService> _logger;

        public GeminiService(HttpClient httpClient, string apiKey, ILogger<GeminiService> logger)
        {
            _httpClient = httpClient;
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _logger = logger;
        }

        public async Task<string> GenerateRecipe(string prompt)
        {
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiUrl}?key={_apiKey}", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            try
            {
                var responseObject = JsonSerializer.Deserialize<JsonElement>(responseBody);
                var candidates = responseObject.GetProperty("candidates");

                if (candidates.GetArrayLength() == 0)
                {
                    _logger.LogError("No candidates found. Response body: {ResponseBody}", responseBody);
                    throw new Exception("No candidates found.");
                }

                var geminiResponseText = candidates[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();

                //cleanse (TODO: move to a separate method)
                geminiResponseText = geminiResponseText.Substring(3, geminiResponseText.Length - 6).Trim();
                // Remove the "json" prefix if it exists
                if (geminiResponseText.StartsWith("json", StringComparison.OrdinalIgnoreCase))
                {
                    geminiResponseText = geminiResponseText.Substring(4).Trim();
                }

                return geminiResponseText;
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "JSON deserialization error. Response body: {ResponseBody}", responseBody);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating the recipe. Response body: {ResponseBody}", responseBody);
                throw;
            }
        }
    }
}
