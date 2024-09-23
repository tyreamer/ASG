using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using ASG.Services;

namespace ASG.Services
{
    public class AuthenticationService(FirebaseService firebaseService, AuthenticationStateProvider authStateProvider)
    {
        private readonly FirebaseService _firebaseService = firebaseService;
        private readonly ASGAuthenticationStateProvider _authStateProvider = authStateProvider as ASGAuthenticationStateProvider
                                 ?? throw new ArgumentException("Invalid AuthenticationStateProvider");

        public async Task SignInWithGoogleAsync()
        {
            var firebaseUser = await _firebaseService.SignInWithGoogleAsync();
            if (firebaseUser != null)
            {
                await _authStateProvider.MarkUserAsAuthenticated(firebaseUser);
            }
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
    }
}
