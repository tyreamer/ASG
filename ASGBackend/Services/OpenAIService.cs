using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ASGShared.Models;

namespace ASGBackend.Services
{
    public class OpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiUrl = "https://api.openai.com/v1/chat/completions";

        public OpenAIService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenAI:ApiKey"];
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<Recipe> GenerateRecipeRecommendation(string userPreferences)
        {
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful assistant that generates recipe recommendations based on user preferences." },
                    new { role = "user", content = $"Generate a recipe recommendation based on these preferences: {userPreferences}. The response should be in JSON format with the following fields: Name, Ingredients (array of strings), Instructions (array of strings), CuisineType, Calories, PrepTime." }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<OpenAIResponse>(responseBody);
                var recipeJson = responseObject.Choices[0].Message.Content;

                // Manually parse and map the response to the Recipe object
                var recipe = JsonSerializer.Deserialize<Recipe>(recipeJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return recipe;
            }

            throw new Exception("Failed to generate recipe recommendation from OpenAI");
        }
    }

    public class OpenAIResponse
    {
        public Choice[] Choices { get; set; }
    }

    public class Choice
    {
        public Message Message { get; set; }
    }

    public class Message
    {
        public string Content { get; set; }
    }
}
