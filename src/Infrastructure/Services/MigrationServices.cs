using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Persistence.Seed;
using Infrastructure.Services.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Infrastructure.Services
{
    public class MigrationServices : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MigrationServices> _logger;
        private readonly IConfiguration _configuration;

        public MigrationServices(IServiceProvider serviceProvider, ILogger<MigrationServices> logger, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            bool hasPendingMigrations = false;

            try
            {
                var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
                if (pendingMigrations.Any())
                {
                    hasPendingMigrations = true;
                    _logger.LogInformation("Applying pending migrations...");
                    await dbContext.Database.MigrateAsync(cancellationToken);
                    _logger.LogInformation("Migrations applied successfully.");
                }
                else
                {
                    _logger.LogInformation("No pending migrations found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while applying migrations.");
            }

            // Kiểm tra xem DB có dữ liệu hay chưa.
            // Ở đây ví dụ kiểm tra bảng Users (hoặc bạn có thể kiểm tra bảng cốt lõi khác).
            bool hasData = dbContext.Users.Any(); // Hoặc bất kỳ bảng nào bạn muốn

            // Chỉ seed nếu:
            //  - Có pending migration vừa được áp dụng, HOẶC
            //  - Chưa có dữ liệu trong DB
            if (hasPendingMigrations || !hasData)
            {
                _logger.LogInformation("Seeding data...");

                // 1) Seed các dữ liệu chung
                var seedResult = await SeedData.Initialize(scope.ServiceProvider);
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.Indented
                };
                string jsonResult = JsonConvert.SerializeObject(seedResult, settings);
                Console.WriteLine(jsonResult);

                // 2) Seed user + hoạt động
                try
                {
                    var seedUser = scope.ServiceProvider.GetRequiredService<SeedUser>();
                    var userSettings = _configuration.GetSection("Seed:Users").Get<UserSettings>();
                    await seedUser.SeedUsersWithActivitiesAsync(
                        userSettings.Password, 
                        userSettings.NumberOfUsers, 
                        userSettings.NumberOfDays
                    );
                    _logger.LogInformation("Users and activities seeded successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while seeding users.");
                }
            }
            else
            {
                _logger.LogInformation("Skipping seeding: No pending migrations and DB already has data.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
