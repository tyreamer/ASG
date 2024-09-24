using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ASG.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using ASGClient;
using MudBlazor.Services;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// Register services
builder.Services.AddScoped<FirebaseService>();
builder.Services.AddScoped<TokenManagerService>();
builder.Services.AddScoped<TokenValidationService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AuthenticationStateProvider, ASGAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddMudServices();
builder.Services.AddAuthorizationCore();
builder.Services.AddHttpClient("ASGClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5050");
});
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ASGClient"));
builder.Services.AddScoped<MealPlanClientService>();
builder.Services.AddMudServices();

await builder.Build().RunAsync();
