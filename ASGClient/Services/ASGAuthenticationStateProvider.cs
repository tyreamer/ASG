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
        private readonly UserClientService _userClientService;

        public ASGAuthenticationStateProvider(ILocalStorageService localStorage, IJSRuntime jsRuntime, UserClientService userClientService)
        {
            _localStorage = localStorage;
            _jsRuntime = jsRuntime;
            _userClientService = userClientService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var firebaseUser = await _localStorage.GetItemAsync<FirebaseUser>("ASGUser");
                if (firebaseUser == null)
                {
                    Console.WriteLine("No user found in local storage.");
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                if (string.IsNullOrEmpty(firebaseUser.DisplayName) || string.IsNullOrEmpty(firebaseUser.Email) || firebaseUser.StsTokenManager == null)
                {
                    Console.WriteLine("User data is missing required fields.");
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                Console.WriteLine($"User Found in local storage: {firebaseUser.DisplayName}");


                //fill out the claims we can...if this is a new user we may not have an id yet
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, firebaseUser.DisplayName),
                    new Claim(ClaimTypes.Email, firebaseUser.Email),
                    new Claim("Token", firebaseUser.StsTokenManager.AccessToken)
                };

                // Retrieve the user from the database to get the GUID
                var user = await _userClientService.GetUserByEmailAsync(firebaseUser.Email);
                if (user == null)
                {
                    Console.WriteLine("User not found in the database.");
                }
                else
                {
                    //This is a registered user
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString() ?? null));
                }

                var identity = new ClaimsIdentity(claims, "firebaseAuth");
                var claimsPrincipal = new ClaimsPrincipal(identity);
                return new AuthenticationState(claimsPrincipal);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving authentication state: {ex.Message}");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }


        public async Task<bool> UserIsAuthenticated(FirebaseUser firebaseUser)
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

            // Retrieve the user from the database to get the GUID
            var user = await _userClientService.GetUserByEmailAsync(firebaseUser.Email);

            if (user == null)
            {
                return false;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, firebaseUser.DisplayName),
                new Claim(ClaimTypes.Email, firebaseUser.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Add the GUID to the claims
                new Claim("Token", firebaseUser.StsTokenManager.AccessToken)
            };

            var identity = new ClaimsIdentity(claims, "firebaseAuth");
            var userPrincipal = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(userPrincipal)));

            return true;
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
                //DB will set ID
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
