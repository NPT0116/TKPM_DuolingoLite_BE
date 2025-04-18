using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.User.Commands.Register;
using Application.Interface;
using Bogus;
using Domain.Entities.Users;
using Domain.Repositories;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Enums;

namespace Infrastructure.Persistence.Seed
{
    public class SeedUser
    {
        private readonly UserRegisterCommandHandler _userRegisterCommandHandler;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IIdentityService _identityService;
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _context;

        public SeedUser(
            UserRegisterCommandHandler userRegisterCommandHandler, 
            IUserRepository userRepository, 
            ApplicationDbContext context,
            RoleManager<IdentityRole<Guid>> roleManager,
            UserManager<ApplicationUser> userManager,
            IIdentityService identityService)
        {
            _userRegisterCommandHandler = userRegisterCommandHandler;
            _userRepository = userRepository;
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _identityService = identityService;
        }

        public async Task SeedUsersWithActivitiesAsync(string password, int numberOfUsers, int numberOfDays)
        {
            var totalUsers = await _userRepository.GetTotalUsersCount();
            if(totalUsers > 0) return;
            // 1. Lấy toàn bộ tên role đã có trong DB trước, giảm số lần gọi SQL
            var existingRoles = _roleManager.Roles
                                            .Select(r => r.Name)
                                            .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // 2. Duyệt qua mọi giá trị enum và tạo nếu còn thiếu
            foreach (var roleName in Enum.GetNames<Domain.Entities.Users.Role>())   // hoặc Enum.GetValues<Role>()
            {
                if (!existingRoles.Contains(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                }
            }
            await _context.SaveChangesAsync();
            // Seed roles


            var faker = new Faker<UserRegisterCommand>()
            .CustomInstantiator(f => new UserRegisterCommand(new UserRegisterDto(
                f.Name.FirstName(),
                f.Name.LastName(),
                f.Internet.Email(),
                f.Internet.UserName(),
                password
            ), null));

            var admin = new ApplicationUser()
            {
                FirstName = "Nguyen",
                LastName = "Hong Quan"
            };

            var (adminResult, adminUserId) = await _identityService.CreateUserAsync(
                "Nguyen",
                "Hong Quan",
                "nquan003@gmail.com",
                "admin",
                "123456"
            );

            if(adminResult.IsSuccess)
            {
                await _context.SaveChangesAsync();
                var adminUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == adminUserId);
                if(adminUser != null)
                    await _userManager.AddToRoleAsync(adminUser, Domain.Entities.Users.Role.Admin.ToString());
            }



            for (int i = 0; i < numberOfUsers; i++)
            {
                var userRegisterCommand = faker.Generate();
                var result = await _userRegisterCommandHandler.Handle(userRegisterCommand, CancellationToken.None);

                if (result.IsSuccess)
                {
                    Console.WriteLine($"User {userRegisterCommand.UserRegisterDto.Email} created with ID {result.Value}");
                    await SeedDailyActivities(result.Value, numberOfDays);
                }
                else
                {
                    Console.WriteLine($"Failed to create user {userRegisterCommand.UserRegisterDto.Email}: {result.Error}");
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedDailyActivities(Guid userId, int numberOfDays)
        {
            var startDate = DateTime.UtcNow.AddDays(-numberOfDays - 1).Date;

            var faker = new Faker<UserActivity>()
                .CustomInstantiator(f => 
                {
                    var date = startDate.AddDays(f.IndexFaker + 1);
                    Console.WriteLine($"Date: {f.IndexFaker}");
                    return UserActivity.Create(
                        userId: userId,
                        date: date,
                        isActive: f.Random.Bool()
                    ).Value;
                });

            // Generate activities with continuous unique dates
            var activities = faker.Generate(numberOfDays);

            foreach (var activity in activities)
            {
                await _userRepository.CreateUserActivity(activity);
                Console.WriteLine($"UserId: {activity.UserId}, Date: {activity.Date}, IsActive: {activity.IsActive}");
            }

        }
    }
}