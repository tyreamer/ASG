using ASGShared.Models;
using System.Net.Http.Json;

public class UserClientService
{
    private readonly HttpClient _httpClient;

    public UserClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> IsEmailRegisteredAsync(string email)
    {
        var response = await _httpClient.GetAsync($"api/users/registered/{email}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        return false;
    }

    public async Task<User?> GetUserAsync(Guid userId)
    {
        var response = await _httpClient.GetAsync($"api/users/{userId}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<User>();
        }

        return null;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var response = await _httpClient.GetAsync($"api/users?email={email}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<User>();
        }

        return null;
    }

    public async Task<User?> CreateUserAsync(User user)
    {
        // Create the user
        var response = await _httpClient.PostAsJsonAsync("api/users", user);

        if (response.IsSuccessStatusCode)
        {
            var createdUser = await response.Content.ReadFromJsonAsync<User>();

            // Update the Preferences with the created user
            if (createdUser != null && user.Preferences == null)
            {
                var preferences = new UserPreferences();
                preferences.UserId = createdUser.Id;

                // Update the user with preferences
                var updateResponse = await _httpClient.PutAsJsonAsync($"api/users/{createdUser.Id}/preferences", preferences);
                if (updateResponse.IsSuccessStatusCode)
                {
                    createdUser.Preferences = await updateResponse.Content.ReadFromJsonAsync<UserPreferences>();
                    return createdUser;
                }
            }

            return createdUser;
        }
        else
        {
            // Log the response for debugging
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
        }

        return null;
    }

    public async Task<User?> UpdateUserAsync(Guid userId, User updatedUser)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/users/{userId}", updatedUser);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<User>();
        }
        return null;
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        var response = await _httpClient.DeleteAsync($"api/users/{userId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<UserPreferences> GetUserPreferencesAsync(Guid userId)
    {
        var response = await _httpClient.GetAsync($"api/users/{userId}/preferences");
        return await response.Content.ReadFromJsonAsync<UserPreferences>() ?? new UserPreferences();
    }

    public async Task<UserPreferences?> UpdateUserPreferencesAsync(Guid userId, UserPreferences updatedPreferences)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/users/{userId}/preferences", updatedPreferences);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<UserPreferences>();
        }
        return null;
    }
}
