using Application;
using Infrastructure;
using Serilog;
using WebApi.Infrastructure;
using Microsoft.OpenApi.Models;
using WebApi.Middlewares;
using WebApi.Swagger;
using WebApi.Utils;
using Domain.Entities.Payment;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("awsSettings.json");
builder.Configuration.AddJsonFile("tkpm-speech-to-text-232c1ce8d4c3.json");
// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(new OptionDtoPolymorphicConverter());
        opts.JsonSerializerOptions.WriteIndented = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DuolingoLite API",
        Version = "v1",
        Description = "API for DuolingoLite"
    });
    // c.OperationFilter<FileUploadOperationFilter>();

    // Adding Authentication for Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Host.UseSerilog((context, configuration) => 
        configuration.ReadFrom.Configuration(context.Configuration));
// check api key exists
string credentialsPath = builder.Configuration["Google:CredentialsPath"];
// connect momo api
builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));


var app = builder.Build();
app.UseCors("AllowAll");

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Log startup information
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Starting application...");
logger.LogInformation($"Environment: {app.Environment.EnvironmentName}");
logger.LogInformation($"Connection string: {builder.Configuration.GetConnectionString("DefaultConnection")}");

// Configure middleware

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Add these lines before app.Run()
logger.LogInformation($"Now listening on: http://localhost:50281");
logger.LogInformation("Application started. Press Ctrl+C to shut down.");
logger.LogInformation($"Content root path: {app.Environment.ContentRootPath}");

app.Run();
