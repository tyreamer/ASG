using Microsoft.JSInterop;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using ASG.Models;
using System.Text.Json;

namespace ASG.Services
{
    public class FirebaseClientService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;

        public FirebaseClientService(IJSRuntime jsRuntime, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
        {
            _jsRuntime = jsRuntime;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
        }

        public async Task<FirebaseUser> SignInWithGoogleAsync()
        {
            var user = await _jsRuntime.InvokeAsync<Dictionary<string, object>>("firebaseAuth.signInWithGoogle");

            if (user == null)
            {
                return null;
            }

            if (!user.TryGetValue("stsTokenManager", out var stsTokenManagerJson) || stsTokenManagerJson == null)
            {
                throw new KeyNotFoundException("The key 'stsTokenManager' was not found in the dictionary.");
            }

            // Deserialize the stsTokenManager using JsonElement
            var stsTokenManagerElement = (JsonElement)stsTokenManagerJson;
            var stsTokenManager = new StsTokenManager
            {
                RefreshToken = stsTokenManagerElement.GetProperty("refreshToken").GetString() ?? throw new InvalidOperationException("RefreshToken is null."),
                AccessToken = stsTokenManagerElement.GetProperty("accessToken").GetString() ?? throw new InvalidOperationException("AccessToken is null."),
                ExpirationTime = stsTokenManagerElement.GetProperty("expirationTime").GetString() ?? throw new InvalidOperationException("ExpirationTime is null.")
            };

            var firebaseUser = new FirebaseUser
            {
                Uid = user["uid"]?.ToString() ?? throw new InvalidOperationException("Uid is null."),
                DisplayName = user["displayName"]?.ToString() ?? string.Empty,
                PhotoURL = user["photoURL"]?.ToString() ?? string.Empty,
                Email = user["email"]?.ToString() ?? throw new InvalidOperationException("Email is null."),
                EmailVerified = bool.Parse(user["emailVerified"]?.ToString() ?? "false"),
                PhoneNumber = user["phoneNumber"]?.ToString() ?? string.Empty,
                IsAnonymous = bool.Parse(user["isAnonymous"]?.ToString() ?? "false"),
                TenantId = user["tenantId"]?.ToString() ?? string.Empty,
                StsTokenManager = stsTokenManager,
                LastLoginAt = user["lastLoginAt"]?.ToString() ?? string.Empty,
                CreatedAt = user["createdAt"]?.ToString() ?? string.Empty
            };

            // Ensure required fields are not null
            if (string.IsNullOrEmpty(firebaseUser.Email) || firebaseUser.StsTokenManager == null || string.IsNullOrEmpty(firebaseUser.StsTokenManager.AccessToken))
            {
                throw new InvalidOperationException("User data is missing required fields.");
            }

            // Save the user to local storage
            await _localStorage.SetItemAsync("ASGUser", firebaseUser);

            var customAuthStateProvider = (ASGAuthenticationStateProvider)_authStateProvider;
            await customAuthStateProvider.UserIsAuthenticated(firebaseUser);

            return firebaseUser;
        }

        public async Task SignOutAsync()
        {
            await _jsRuntime.InvokeVoidAsync("firebaseAuth.signOut");
            var customAuthStateProvider = (ASGAuthenticationStateProvider)_authStateProvider;
            await customAuthStateProvider.MarkUserAsLoggedOut();
        }

        public async Task<Dictionary<string, string>> GetUserFromLocalStorageAsync()
        {
            var user = await _localStorage.GetItemAsync<Dictionary<string, string>>("ASGUser");
            if (user == null)
            {
                Console.WriteLine("Firebase: No user found in local storage.");
            }
            return user;
        }
    }
}
