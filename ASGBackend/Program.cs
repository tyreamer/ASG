using ASGBackend.Hubs;
using ASGBackend.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);

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

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<FirebaseService>();
builder.Services.AddSignalR();

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
app.MapHub<ASGHub>("/ASGhub");

app.Run();
