using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using ASGShared.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Threading.Tasks;
using ASG.Models;

namespace ASG.Services
{
    public class AuthenticationService
    {
        private readonly FirebaseClientService _firebaseClientService;
        private readonly ASGAuthenticationStateProvider _authStateProvider;
        private readonly NavigationManager _navigation;
        private readonly UserClientService _userClientService;

        public AuthenticationService(FirebaseClientService firebaseClientService, AuthenticationStateProvider authStateProvider, NavigationManager navigation, UserClientService userClientService)
        {
            _firebaseClientService = firebaseClientService;
            _authStateProvider = authStateProvider as ASGAuthenticationStateProvider
                                 ?? throw new ArgumentException("Invalid AuthenticationStateProvider");
            _navigation = navigation;
            _userClientService = userClientService;
        }

        public async Task SignInWithGoogleAsync()
        {
            var firebaseUser = await _firebaseClientService.SignInWithGoogleAsync();

            if (firebaseUser == null)
            {
                // Attempt to get user data from localStorage if offline
                var userData = await _firebaseClientService.GetUserFromLocalStorageAsync();
                if (userData != null)
                {
                    firebaseUser = new FirebaseUser
                    {
                        Uid = userData["uid"],
                        DisplayName = userData["displayName"],
                        PhotoURL = userData["photoURL"],
                        Email = userData["email"],
                        EmailVerified = bool.Parse(userData["emailVerified"]),
                        PhoneNumber = userData["phoneNumber"],
                        IsAnonymous = bool.Parse(userData["isAnonymous"]),
                        TenantId = userData["tenantId"]
                    };
                }
            }

            if (firebaseUser != null)
            {
                var isAuthenticated = await _authStateProvider.UserIsAuthenticated(firebaseUser);
                if (isAuthenticated)
                {
                    var isRegistered = await _userClientService.IsEmailRegisteredAsync(firebaseUser.Email);
                    if (isRegistered)
                    {
                        _navigation.Refresh();
                        return;
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

        public async Task<Guid?> GetAuthenticatedUserIdAsync()
        {
            var authState = await GetAuthenticationStateAsync();
            var userClaims = authState.User;

            if (userClaims.Identity != null && userClaims.Identity.IsAuthenticated)
            {
                var userIdClaim = userClaims.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (Guid.TryParse(userIdClaim, out var userId))
                {
                    return userId;
                }
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
