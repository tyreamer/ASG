using System.Security.Claims;
using Blazored.LocalStorage;
using ASG.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using ASGShared.Models;

namespace ASG.Services
{
    public class ASGAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly IJSRuntime _jsRuntime;

        public ASGAuthenticationStateProvider(ILocalStorageService localStorage, IJSRuntime jsRuntime)
        {
            _localStorage = localStorage;
            _jsRuntime = jsRuntime;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var user = await _localStorage.GetItemAsync<FirebaseUser>("ASGUser");
                if (user == null)
                {
                    Console.WriteLine("No user found in local storage.");
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                if (string.IsNullOrEmpty(user.DisplayName) || string.IsNullOrEmpty(user.Email) || user.StsTokenManager == null)
                {
                    Console.WriteLine("User data is missing required fields.");
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                Console.WriteLine($"User Found: {user.DisplayName}");

                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.DisplayName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("Token", user.StsTokenManager.AccessToken)
                }, "firebaseAuth");

                var claimsPrincipal = new ClaimsPrincipal(identity);
                return new AuthenticationState(claimsPrincipal);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving authentication state: {ex.Message}");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public async Task<User> MarkUserAsAuthenticated(FirebaseUser firebaseUser)
        {
            if (firebaseUser == null)
            {
                throw new ArgumentNullException(nameof(firebaseUser));
            }

            if (string.IsNullOrEmpty(firebaseUser.Email))
            {
                throw new ArgumentNullException(nameof(firebaseUser.Email));
            }

            // Store the user in local storage
            await _localStorage.SetItemAsync("ASGUser", firebaseUser);

            var claims = new List<Claim>
            {
                new (ClaimTypes.Name, firebaseUser.DisplayName),
                new (ClaimTypes.Email, firebaseUser.Email),
                new ("Token", firebaseUser.StsTokenManager.AccessToken)
            };

            var identity = new ClaimsIdentity(claims, "firebaseAuth");
            var userPrincipal = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(userPrincipal)));

            // Convert FirebaseUser to User and return
            return ConvertFirebaseUserToUser(firebaseUser);
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _localStorage.RemoveItemAsync("ASGUser");
            var identity = new ClaimsIdentity();
            var claimsPrincipal = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        private User ConvertFirebaseUserToUser(FirebaseUser firebaseUser)
        {
            return new User
            {
                Id = firebaseUser.Uid,
                DisplayName = firebaseUser.DisplayName,
                Email = firebaseUser.Email,
                IsAuthenticated = true,
                Claims = new Dictionary<string, string>
                {
                    { ClaimTypes.Name, firebaseUser.DisplayName },
                    { ClaimTypes.Email, firebaseUser.Email },
                    { "Token", firebaseUser.StsTokenManager.AccessToken }
                }
            };
        }
    }
}
