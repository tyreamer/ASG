using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using ASG.Services;
using ASGShared.Models;

namespace ASG.Services
{
    public class AuthenticationService
    {
        private readonly FirebaseService _firebaseService;
        private readonly ASGAuthenticationStateProvider _authStateProvider;

        public AuthenticationService(FirebaseService firebaseService, AuthenticationStateProvider authStateProvider)
        {
            _firebaseService = firebaseService;
            _authStateProvider = authStateProvider as ASGAuthenticationStateProvider
                                 ?? throw new ArgumentException("Invalid AuthenticationStateProvider");
        }

        public async Task<User?> SignInWithGoogleAsync()
        {
            var firebaseUser = await _firebaseService.SignInWithGoogleAsync();
            if (firebaseUser != null)
            {
                return await _authStateProvider.MarkUserAsAuthenticated(firebaseUser);
            }

            return null;
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
    }
}
