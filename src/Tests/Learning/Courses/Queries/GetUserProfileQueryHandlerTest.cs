using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.User.Queries.GetMe;
using Application.Features.User.Queries.GetUserProfile;
using Application.Interface;
using Domain.Entities.Users;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using SharedKernel;
using Xunit;

namespace Application.Tests.Features.User.Queries
{
    public class GetUserProfileQueryHandlerTests
    {
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly GetUserProfileQueryHandler _handler;

        public GetUserProfileQueryHandlerTests()
        {
            _identityServiceMock = new Mock<IIdentityService>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new GetUserProfileQueryHandler(_identityServiceMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserIsNotLoggedIn()
        {
            // Arrange
            _identityServiceMock.Setup(s => s.GetCurrentUserAsync())
                                .ReturnsAsync((UserDto?)null);

            var query = new GetUserProfileQuery();
            
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(UserError.UnauthorizedUser);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserProfileNotFound()
        {
            // Arrange
            var user = new UserDto() { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Email = "john@example.com" };

            _identityServiceMock.Setup(s => s.GetCurrentUserAsync())
                                .ReturnsAsync(user);

            _userRepositoryMock.Setup(r => r.GetUserProfileById(user.Id))
                               .ReturnsAsync((UserProfile?)null);

            var query = new GetUserProfileQuery();
            
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(UserError.UserProfileNotFound(user.Id));
        }

        [Fact]
        public async Task Handle_ShouldReturnUserProfile_WhenUserProfileExists()
        {
            // Arrange
            var user = new UserDto() { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Email = "john@example.com" };
            var userProfile = UserProfile.Create(user.Id, user.Email, user.UserName, user.FirstName, user.LastName, null, null);
            var userStats = UserStats.Create(user.Id);
            var userActivities = new List<UserActivity>
            {
                UserActivity.Create(user.Id, DateTime.UtcNow, true).Value,
                UserActivity.Create(user.Id, DateTime.UtcNow.AddDays(-1), false).Value
            };

            _identityServiceMock.Setup(s => s.GetCurrentUserAsync())
                                .ReturnsAsync(user);

            _userRepositoryMock.Setup(r => r.GetUserProfileById(user.Id))
                               .ReturnsAsync(userProfile.Value);

            _userRepositoryMock.Setup(r => r.GetUserStatsById(user.Id))
                               .ReturnsAsync(userStats.Value);

            _userRepositoryMock.Setup(r => r.GetUserActivitiesWithinDateRangeByUserId(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                               .ReturnsAsync(userActivities);

            var query = new GetUserProfileQuery();
            
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be(user.Id);
            result.Value.FirstName.Should().Be("John");
            result.Value.LastName.Should().Be("Doe");
            result.Value.Email.Should().Be("john@example.com");
            result.Value.UserStats.Should().Be(userStats.Value);
            result.Value.UserActivities.Should().BeEquivalentTo(userActivities);
        }
    }
}
