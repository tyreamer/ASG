using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using ASGShared.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Threading.Tasks;

namespace ASG.Services
{
    public class AuthenticationService
    {
        private readonly FirebaseService _firebaseService;
        private readonly ASGAuthenticationStateProvider _authStateProvider;
        private readonly NavigationManager _navigation;
        private readonly UserClientService _userClientService;

        public AuthenticationService(FirebaseService firebaseService, AuthenticationStateProvider authStateProvider, NavigationManager navigation, UserClientService userClientService)
        {
            _firebaseService = firebaseService;
            _authStateProvider = authStateProvider as ASGAuthenticationStateProvider
                                 ?? throw new ArgumentException("Invalid AuthenticationStateProvider");
            _navigation = navigation;
            _userClientService = userClientService;
        }

        public async Task SignInWithGoogleAsync()
        {
            var firebaseUser = await _firebaseService.SignInWithGoogleAsync();
            if (firebaseUser != null)
            {
                var isAuthenticated = await _authStateProvider.UserIsAuthenticated(firebaseUser);
                if (isAuthenticated)
                {
                    var isRegistered = await _userClientService.IsEmailRegisteredAsync(firebaseUser.Email);
                    if (isRegistered)
                    {
                        _navigation.Refresh();
                    }
                }

                _navigation.NavigateTo($"/signup?name={Uri.EscapeDataString(firebaseUser.DisplayName)}&email={Uri.EscapeDataString(firebaseUser.Email)}");
            }
        }

        public async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return await _authStateProvider.GetAuthenticationStateAsync();
        }

        public static async Task CheckAuthenticationStateAsync(Task<AuthenticationState> authStateTask)
        {
            var authState = await authStateTask;
            var user = authState.User;

            if (user?.Identity?.IsAuthenticated ?? false)
            {
                var userName = user.FindFirst(ClaimTypes.Name)?.Value;
                Console.WriteLine($"User is authenticated: {userName}");
            }
            else
            {
                Console.WriteLine("User is not authenticated");
            }
        }

        public async Task<string?> GetAuthenticatedUserEmailAsync()
        {
            var authState = await GetAuthenticationStateAsync();
            var userClaims = authState.User;

            if (userClaims.Identity != null && userClaims.Identity.IsAuthenticated)
            {
                return userClaims.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
            }

            return null;
        }

        public async Task HandleAuthenticationAsync(Task<AuthenticationState> authStateTask)
        {
            if (authStateTask != null)
            {
                await CheckAuthenticationStateAsync(authStateTask);
            }

            var authState = await GetAuthenticationStateAsync();
            var userClaims = authState.User;

            if (userClaims.Identity != null && userClaims.Identity.IsAuthenticated)
            {
                var email = userClaims.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email) || !await _userClientService.IsEmailRegisteredAsync(email))
                {
                    _navigation.NavigateTo("/signup");
                }
            }
            else
            {
                _navigation.NavigateTo("/");
            }
        }

        public async Task<bool> IsUserRegisteredAsync(string email)
        {
            return await _userClientService.IsEmailRegisteredAsync(email);
        }

        public async Task LogoutAsync()
        {
            await _authStateProvider.MarkUserAsLoggedOut();
            _navigation.NavigateTo("/");
        }
    }
}
