using Application;
using Infrastructure;
using Serilog;
using WebApi.Infrastructure;
using Microsoft.OpenApi.Models;
using WebApi.Middlewares;
using WebApi.Swagger;
using WebApi.Utils;
using Domain.Entities.Payment;
using Infrastructure.Hubs;
using Application.Interfaces;
using Application.Notifications.Commands.SendNotification;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("awsSettings.json");
builder.Configuration.AddJsonFile("tkpm-speech-to-text-232c1ce8d4c3.json");

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllWithCredentials", policy =>
    {
        policy
            .SetIsOriginAllowed(_ => true) // Thay thế cho AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Add controllers with JSON options
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(new OptionDtoPolymorphicConverter());
        
        opts.JsonSerializerOptions.WriteIndented = true;
    });

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DuolingoLite API",
        Version = "v1",
        Description = "API for DuolingoLite"
    });

    // Adding Authentication for Swagger với scheme "MyBearer"
    c.AddSecurityDefinition("MyBearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "MyBearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "MyBearer"
                }
            },
            new string[] { }
        }
    });
});

// Các service khác
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Host.UseSerilog((context, configuration) => 
        configuration.ReadFrom.Configuration(context.Configuration));

// Check API key, cấu hình Momo API, v.v.
string credentialsPath = builder.Configuration["Google:CredentialsPath"];
builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));

// Cấu hình Authentication sử dụng scheme "MyBearer"
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "MyBearer";
    options.DefaultChallengeScheme = "MyBearer";
})
.AddJwtBearer("MyBearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "DuolingoLite",
        ValidAudience = "DuolingoLite",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"])),
        NameClaimType = "sub"
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificationHub"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});


builder.Services.AddSignalR();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SendNotificationCommand).Assembly));

builder.Services.AddScoped<INotificationService, Infrastructure.Services.NotificationService>();
builder.Services.AddScoped<INotificationHub, NotificationHub>();
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

var app = builder.Build();

// Cấu hình middleware theo thứ tự: CORS -> Authentication -> Authorization -> Middleware custom
app.UseCors("AllowAllWithCredentials");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Log startup information
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Starting application...");
logger.LogInformation($"Environment: {app.Environment.EnvironmentName}");
logger.LogInformation($"Connection string: {builder.Configuration.GetConnectionString("DefaultConnection")}");

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// HTTPS redirection
app.UseHttpsRedirection();

app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

logger.LogInformation($"Now listening on: http://localhost:50281");
logger.LogInformation("Application started. Press Ctrl+C to shut down.");
logger.LogInformation($"Content root path: {app.Environment.ContentRootPath}");

app.Run();
