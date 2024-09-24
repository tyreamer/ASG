using ASGShared.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ASG.Services
{
    public class UserClientService
    {
        private readonly HttpClient _httpClient;

        public UserClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> IsUserRegisteredAsync(string email)
        {
            var response = await _httpClient.GetAsync($"api/users/registered/{email}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }
            return false;
        }

        public async Task<User?> GetUserAsync(string email)
        {
            var response = await _httpClient.GetAsync($"api/users/{email}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<User>();
            }
            return null;
        }

        public async Task<User?> CreateUserAsync(User user)
        {
            var response = await _httpClient.PostAsJsonAsync("api/users", user);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<User>();
            }
            return null;
        }

        public async Task<User?> UpdateUserAsync(string id, User updatedUser)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/users/{id}", updatedUser);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<User>();
            }
            return null;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"api/users/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
