using Application;
using Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Host.UseSerilog((context, configuration) => 
        configuration.ReadFrom.Configuration(context.Configuration));
var app = builder.Build();

// Log startup information
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Starting application...");
logger.LogInformation($"Environment: {app.Environment.EnvironmentName}");
logger.LogInformation($"Connection string: {builder.Configuration.GetConnectionString("DefaultConnection")}");

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Add these lines before app.Run()
logger.LogInformation($"Now listening on: http://localhost:50281");
logger.LogInformation("Application started. Press Ctrl+C to shut down.");
logger.LogInformation($"Content root path: {app.Environment.ContentRootPath}");

app.Run();
