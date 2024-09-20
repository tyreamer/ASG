using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ASG.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using ASGClient;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// Register services
builder.Services.AddScoped<FirebaseService>();
builder.Services.AddScoped<TokenManagerService>();
builder.Services.AddScoped<TokenValidationService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthenticationStateProvider, ASGAuthenticationStateProvider>();
builder.Services.AddMudServices();
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
