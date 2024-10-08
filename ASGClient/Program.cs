using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ASG.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using ASGClient;
using MudBlazor.Services;
using System.Net.Http;
using System.ComponentModel.Design;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// Register services
builder.Services.AddScoped<FirebaseClientService>();
builder.Services.AddScoped<TokenManagerService>();
builder.Services.AddScoped<TokenValidationService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthenticationStateProvider, ASGAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddMudServices();
builder.Services.AddAuthorizationCore();

var isDevelopment = builder.HostEnvironment.IsDevelopment();

builder.Services.AddHttpClient("ASGClient", client =>
{
    client.BaseAddress = isDevelopment
    ? new Uri("http://localhost:5050")  // For local development
    : new Uri(builder.HostEnvironment.BaseAddress);  // For production
});


builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ASGClient"));
builder.Services.AddScoped<MealPlanClientService>();
builder.Services.AddScoped<UserClientService>();
builder.Services.AddMudBlazorDialog();

await builder.Build().RunAsync();
