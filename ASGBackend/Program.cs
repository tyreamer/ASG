using ASG.Services;
using ASGBackend.Agents;
using ASGBackend.Data;
using ASGBackend.Interfaces;
using ASGBackend.Repositories;
using ASGBackend.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

// Get the path to the Firebase Admin SDK JSON file from the environment variable
var firebaseCredentialPath = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");

if (string.IsNullOrEmpty(firebaseCredentialPath))
{
    throw new InvalidOperationException("The GOOGLE_APPLICATION_CREDENTIALS environment variable is not set.");
}

// Initialize Firebase Admin SDK
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(firebaseCredentialPath)
});

// Retrieve the Gemini API key from environment variables
var geminiApiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
if (string.IsNullOrEmpty(geminiApiKey))
{
    throw new InvalidOperationException("Gemini API key is not set in environment variables.");
}

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FirebaseService>();

// Configure HttpClient for GeminiService
builder.Services.AddHttpClient<GeminiService>();

// Register GeminiService with the API key
builder.Services.AddSingleton<GeminiService>(provider =>
{
    var httpClient = provider.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(GeminiService));
    var logger = provider.GetRequiredService<ILogger<GeminiService>>();
    return new GeminiService(httpClient, geminiApiKey, logger);
});

builder.Services.AddSingleton<UserClusteringAgent>();
builder.Services.AddScoped<MealPlanService>();
builder.Services.AddScoped<UserService>();

// Register repositories
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMealPlanRepository, MealPlanRepository>(); // Add this line

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") + ";TrustServerCertificate=True"));
//TODO: update with cert

// Register AIAgentService with dependencies
builder.Services.AddHttpClient();
builder.Services.AddSingleton<AIAgentService>();

builder.Services.AddControllers();

// Configure the URLs and ports based on the environment
if (builder.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls("http://localhost:5050", "https://localhost:5051");
}
else
{
    builder.WebHost.UseUrls("http://*:80", "https://*:443");
}

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Seed initial data to DB
//using (var scope = app.Services.CreateScope())
//{
//    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//    DataSeeder.SeedInitialData(dbContext);
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.UseCors("AllowAll");
app.MapControllers();

app.Run();
