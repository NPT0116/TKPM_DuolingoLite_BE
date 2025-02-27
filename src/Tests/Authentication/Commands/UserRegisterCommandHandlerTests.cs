using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Application.Common.Interface;
using Application.Common.Settings;
using Application.Features.Media.Commands.Upload;
using Application.Features.User.Commands.Register;
using Application.Interface;
using Castle.Core.Configuration;
using Domain.Entities.Media;
using Domain.Entities.Subscriptions;
using Domain.Entities.Users;
using Domain.Entities.Users.Constants;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using SharedKernel;

namespace Tests.Authentication.Commands
{
    public class UserRegisterCommandHandlerTests
    {
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IApplicationDbContext> _applicationDbContextMock;
        private readonly Mock<IMediaStorageService> _mediaStorageServiceMock;
        private readonly Mock<IMediaRepository> _mediaRepositoryMock;
        private readonly UserRegisterCommandHandler userRegisterCommandHandler;
        private readonly MediaSettings _mediaSettings;
        
        public UserRegisterCommandHandlerTests()
        {
            _identityServiceMock = new Mock<IIdentityService>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _applicationDbContextMock = new Mock<IApplicationDbContext>();
            _mediaStorageServiceMock = new Mock<IMediaStorageService>();
            _mediaSettings = new MediaSettings();
            _mediaRepositoryMock = new Mock<IMediaRepository>();

            userRegisterCommandHandler = new UserRegisterCommandHandler(
                _identityServiceMock.Object,
                _userRepositoryMock.Object,
                _applicationDbContextMock.Object,
                _mediaStorageServiceMock.Object,
                _mediaSettings
            );
        }

        public static UserRegisterDto CreateValidUserRegisterDto(
            string firstName = "John",
            string lastName = "Doe",
            string email = "john.doe@example.com",
            string userName = "Username",
            string password = "Password123")
        {
            return new UserRegisterDto(firstName, lastName, email, userName, password);
        }


        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenEmailAlreadyExists()
        {
            _identityServiceMock.Setup(x => x.UseEmailExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            var userRegisterDto = new UserRegisterDto(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            );

            var userRegisterCommand = new UserRegisterCommand(userRegisterDto, null);
            var result = await userRegisterCommandHandler.Handle(userRegisterCommand, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(UserError.EmailNotUnique);
        } 

        [Fact]
        public async Task Handle_Should_ReturnFailure_WhenUsernameAlreadyExists()
        {
            _identityServiceMock.Setup(x => x.UserNameExistsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            var userRegisterDto = new UserRegisterDto(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            );

            var userRegisterCommand = new UserRegisterCommand(userRegisterDto, null);
            var result = await userRegisterCommandHandler.Handle(userRegisterCommand, CancellationToken.None);

            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(UserError.UserNameNotUnique);
        }

        public static IEnumerable<object[]> UserNameTestData
        {
            get
            {
                yield return new object[] { "", false }; // empty string
                yield return new object[] { new string('a', UserConstants.MINIMUM_USERNAME_LENGTH - 1), false }; // too short
                yield return new object[] { new string('a', UserConstants.MAXIMUM_USERNAME_LENGTH), true }; // exactly minimum
                yield return new object[] { new string('a', (UserConstants.MINIMUM_USERNAME_LENGTH + UserConstants.MAXIMUM_USERNAME_LENGTH) / 2)
, true }; // a valid case
                yield return new object[] { new string('a', UserConstants.MAXIMUM_USERNAME_LENGTH + 1), false }; // too long
            }
        }

            [Theory]
            [MemberData(nameof(UserNameTestData))]
            public void ValidateUserName_Should_EnforceLengthConstraints(string username, bool expectedValidity)
            {
                // Arrange
                var userRegisterDtoBuilder = new UserRegisterDtoBuilder();
                var userRegisterDto = userRegisterDtoBuilder
                    .With(u => u.UserName, username)
                    .Build();
                var command = new UserRegisterCommand(userRegisterDto, null);
                var validator = new UserRegisterCommandValidator();

                // Act
                var result = validator.Validate(command);

                // Assert
                Assert.Equal(expectedValidity, result.IsValid);
            }

        public static IEnumerable<object[]> FirstNameTestData
        {
            get
            {
                yield return new object[] { "", false }; // empty string
                yield return new object[] { new string('a', UserConstants.MAXIMUM_FIRSTNAME_LENGTH), true }; // exactly minimum
                yield return new object[] { "AValidUserName", true }; // a valid case
                yield return new object[] { new string('a', UserConstants.MAXIMUM_FIRSTNAME_LENGTH + 1), false }; // too long
            }
        }

        [Theory]
        [MemberData(nameof(FirstNameTestData))]

        public void ValidateFirstName_Should_EnforceLengthConstraints(string firstName, bool expectedValidity)
        {
            var userRegisterDtoBuilder = new UserRegisterDtoBuilder();
            // Arrange
            var userRegisterDto = userRegisterDtoBuilder
                .With(u => u.FirstName, firstName)
                .Build();
            var command = new UserRegisterCommand(userRegisterDto, null);
            var validator = new UserRegisterCommandValidator();

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.Equal(expectedValidity, result.IsValid);
            if(!result.IsValid)
            {
                result.Errors.Should().Contain(e => e.PropertyName.Contains("FirstName"));
            }
        }

        public static IEnumerable<object[]> LastNameTestData
        {
            get
            {
                yield return new object[] { "", false }; // empty string
                yield return new object[] { new string('a', UserConstants.MAXIMUM_FIRSTNAME_LENGTH), true }; // exactly minimum
                yield return new object[] { new string('a', UserConstants.MAXIMUM_FIRSTNAME_LENGTH - 1), true }; // a valid case
                yield return new object[] { new string('a', UserConstants.MAXIMUM_FIRSTNAME_LENGTH + 1), false }; // too long
            }
        }

        [Theory]
        [MemberData(nameof(LastNameTestData))]

        public void ValidateLastName_Should_EnforceLengthConstraints(string lastname, bool expectedValidity)
        {
            // Arrange
            var userRegisterDtoBuilder = new UserRegisterDtoBuilder();
            // Arrange
            var userRegisterDto = userRegisterDtoBuilder
                .With(u => u.LastName, lastname)
                .Build();
            var command = new UserRegisterCommand(userRegisterDto, null);
            var validator = new UserRegisterCommandValidator();

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.Equal(expectedValidity, result.IsValid);
            if(!result.IsValid)
            {
                result.Errors.Should().Contain(e => e.PropertyName.Contains("LastName"));
            }
        }

        public static IEnumerable<object[]> PasswordTestData
        {
            get
            {
                yield return new object[] { "", false }; // empty string
                yield return new object[] { new string('a', UserConstants.MINIMUM_PASSWORD_LENGTH - 1), false };
                yield return new object[] { new string('a', UserConstants.MINIMUM_PASSWORD_LENGTH), true }; // exactly minimum
                yield return new object[] { new string('a', (UserConstants.MAXIMUM_PASSWORD_LENGTH + UserConstants.MINIMUM_PASSWORD_LENGTH) / 2), true }; // a valid case
                yield return new object[] { new string('a', UserConstants.MAXIMUM_PASSWORD_LENGTH + 1), false }; // too long
            }
        }

        [Theory]
        [MemberData(nameof(PasswordTestData))]

        public void ValidatePassword_Should_EnforceLengthConstraints(string password, bool expectedValidity)
        {
            // Arrange
            var userRegisterDtoBuilder = new UserRegisterDtoBuilder();
            // Arrange
            var userRegisterDto = userRegisterDtoBuilder
                .With(u => u.Password, password)
                .Build();
            var command = new UserRegisterCommand(userRegisterDto, null);
            var validator = new UserRegisterCommandValidator();

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.Equal(expectedValidity, result.IsValid);
            if(!result.IsValid)
            {
                result.Errors.Should().Contain(e => e.PropertyName.Contains("Password"));
            }
        }

        public static IEnumerable<object[]> EmailTestData
        {
            get
            {
                // 1. Empty string: should be invalid.
                yield return new object[] { "", false };
                
                // 2. Plain address without '@': invalid.
                yield return new object[] { "plainaddress", false };
                
                // 3. Valid email with standard format.
                yield return new object[] { "email@domain.com", true };
                
                // 4. Email missing a top-level domain: invalid.
                yield return new object[] { "email@domain", false };
                
                // 5. Email with double dots in the domain: invalid.
                yield return new object[] { "email@domain..com", false };
                
                // 6. Email with a dot in the local part: valid.
                yield return new object[] { "firstname.lastname@domain.com", true };
                
                // 7. Email with a subdomain: valid.
                yield return new object[] { "email@subdomain.domain.com", true };
                
                // 8. Email with a plus tag: valid.
                yield return new object[] { "email+tag@domain.com", true };
                
                // 9. Email with an invalid domain format (domain starts with a hyphen): invalid.
                yield return new object[] { "email@-domain.com", false };
                
                // 10. Email with trailing whitespace: invalid.
                yield return new object[] { "email@domain.com ", false };
            }
        }


        [Theory]
        [MemberData(nameof(EmailTestData))]

        public void ValidateEmail_Should_EnforceLengthConstraints(string email, bool expectedValidity)
        {
            // Arrange
            var userRegisterDtoBuilder = new UserRegisterDtoBuilder();
            // Arrange
            var userRegisterDto = userRegisterDtoBuilder
                .With(u => u.Email, email)
                .Build();
            var command = new UserRegisterCommand(userRegisterDto, null);
            var validator = new UserRegisterCommandValidator();

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.Equal(expectedValidity, result.IsValid);
            if(!result.IsValid)
            {
                result.Errors.Should().Contain(e => e.PropertyName.Contains("Email"));
            }
        }

        // 1. Email already exists => should return failure.
        [Fact]
        public async Task Handle_Should_ReturnFailure_When_EmailAlreadyExists()
        {
            // Arrange
            var userRegisterDtoBuilder = new UserRegisterDtoBuilder();
            var dto = userRegisterDtoBuilder
                .Build();
            var command = new UserRegisterCommand(dto, null);

            _identityServiceMock.Setup(s => s.UseEmailExistsAsync(dto.Email))
                .ReturnsAsync(true);

            // Act
            var result = await userRegisterCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(UserError.EmailNotUnique);
        }

        // 2. User name already exists => should return failure.
        [Fact]
        public async Task Handle_Should_ReturnFailure_When_UserNameAlreadyExists()
        {
            // Arrange
            var userRegisterDtoBuilder = new UserRegisterDtoBuilder();
            var dto = userRegisterDtoBuilder
                .Build();
            var command = new UserRegisterCommand(dto, null);

            _identityServiceMock.Setup(s => s.UseEmailExistsAsync(dto.Email))
                .ReturnsAsync(false);
            _identityServiceMock.Setup(s => s.UserNameExistsAsync(dto.UserName))
                .ReturnsAsync(true);

            // Act
            var result = await userRegisterCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(UserError.UserNameNotUnique);
        }

        // 3. Identity service CreateUser failure => should return failure.
        [Fact]
        public async Task Handle_Should_ReturnFailure_When_CreateUser_Fails()
        {
            // Arrange
            var userRegisterDtoBuilder = new UserRegisterDtoBuilder();
            var dto = userRegisterDtoBuilder
                .Build();
            var command = new UserRegisterCommand(dto, null);

            _identityServiceMock.Setup(s => s.UseEmailExistsAsync(dto.Email))
                .ReturnsAsync(false);
            _identityServiceMock.Setup(s => s.UserNameExistsAsync(dto.UserName))
                .ReturnsAsync(false);
            // Simulate failure in creating user.
            (Result, Guid) createUserResult = new (Result.Failure<Guid>(UserError.UserNameNotUnique), Guid.Empty);
            _identityServiceMock.Setup(s => s.CreateUserAsync(dto.FirstName, dto.LastName, dto.Email, dto.UserName, dto.Password))
                .ReturnsAsync(createUserResult);

            // Act
            var result = await userRegisterCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        

        // 5. Failure during Avatar Upload due to invalid file type.
        [Fact]
        public async Task Handle_Should_ReturnFailure_When_AvatarFileTypeIsInvalid()
        {
            // Arrange: set up a DTO with an avatar upload request having an invalid content type.
            var invalidContentType = "application/pdf";
            var avatarRequest = new AvatarUploadRequest(new byte[10], "file.pdf", invalidContentType);
            var dto = new UserRegisterDto("John", "Doe", "john.doe@example.com", "Username", "Password123");
            var command = new UserRegisterCommand(dto, avatarRequest);

            _identityServiceMock.Setup(s => s.UseEmailExistsAsync(dto.Email)).ReturnsAsync(false);
            _identityServiceMock.Setup(s => s.UserNameExistsAsync(dto.UserName)).ReturnsAsync(false);
            (Result, Guid) createUserResult = new (Result.Success(Guid.NewGuid()), Guid.NewGuid());
            _identityServiceMock.Setup(s => s.CreateUserAsync(dto.FirstName, dto.LastName, dto.Email, dto.UserName, dto.Password))
                .ReturnsAsync(createUserResult);

            // The GetMediaType check in the handler will evaluate the content type.
            // Our expectation is that Media.GetMediaType(invalidContentType).Value != MediaType.Image, so it should fail.
            // Act
            var result = await userRegisterCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(MediaError.InvalidFileType());
        }

        // 6. Failure during Avatar Upload process (upload failure).
        [Fact]
        public async Task Handle_Should_ReturnFailure_When_AvatarUploadFails()
        {
            // Arrange: valid avatar file type but simulate upload failure.
            var validContentType = "image/png";
            var avatarRequest = new AvatarUploadRequest(new byte[10], "avatar.png", validContentType);
            var dto = new UserRegisterDto("John", "Doe", "john.doe@example.com", "Username", "Password123");
            var command = new UserRegisterCommand(dto, avatarRequest);

            _identityServiceMock.Setup(s => s.UseEmailExistsAsync(dto.Email)).ReturnsAsync(false);
            _identityServiceMock.Setup(s => s.UserNameExistsAsync(dto.UserName)).ReturnsAsync(false);
            var userId = Guid.NewGuid();
            (Result, Guid) createUserResult = new (Result.Success(userId), userId);
            _identityServiceMock.Setup(s => s.CreateUserAsync(dto.FirstName, dto.LastName, dto.Email, dto.UserName, dto.Password))
                .ReturnsAsync(createUserResult);

            // Simulate successful media type check.
            // Simulate failure in the media storage service.
            _mediaStorageServiceMock.Setup(s => s.UploadFileAsync(
                    It.IsAny<MediaUploadRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure<Domain.Entities.Media.Media>(MediaError.InvalidFileType()));

            // Act
            var result = await userRegisterCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(MediaError.InvalidFileType());
        }

        // 8. Successful registration
        [Fact]
        public async Task Handle_Should_ReturnUserId_When_AllOperationsSucceed()
        {
            // Arrange
            var userRegisterDtoBuilder = new UserRegisterDtoBuilder();
            var dto = userRegisterDtoBuilder
                .Build();
            var command = new UserRegisterCommand(dto, null);
            var userId = Guid.NewGuid();

            _identityServiceMock.Setup(s => s.UseEmailExistsAsync(dto.Email)).ReturnsAsync(false);
            _identityServiceMock.Setup(s => s.UserNameExistsAsync(dto.UserName)).ReturnsAsync(false);
            (Result, Guid) createUserResult = new (Result.Success(userId), userId);
            _identityServiceMock.Setup(s => s.CreateUserAsync(dto.FirstName, dto.LastName, dto.Email, dto.UserName, dto.Password))
                .ReturnsAsync(createUserResult);

            // Assume UserActivity, UserStats, and UserProfile creation all succeed.
            // For testing, we assume their static Create methods return success.
            // If these are static factories, consider wrapping them for testing or use test doubles.
            // Here we assume they work as expected.

            // Setup repository mocks to simply complete the tasks.
            _userRepositoryMock
                .Setup(r => r.CreateUserActivity(It.IsAny<UserActivity>()))
                .ReturnsAsync(UserActivity.Create(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<bool>()).Value);

            _userRepositoryMock
                .Setup(r => r.CreateUserStats(It.IsAny<UserStats>()))
                .ReturnsAsync(UserStats.Create(It.IsAny<Guid>()).Value);

            _userRepositoryMock
                .Setup(r => r.CreateUserProfile(It.IsAny<UserProfile>()))
                .ReturnsAsync(UserProfile.Create(It.IsAny<Guid>(), dto.Email, dto.UserName, dto.FirstName, dto.LastName, It.IsAny<Media>(), It.IsAny<Subscription?>()).Value);

            _applicationDbContextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await userRegisterCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(userId);
        }

    }

    public class UserRegisterDtoBuilder
{
    private string _firstName = "John";
    private string _lastName = "Doe";
    private string _email = "john.doe@example.com";
    private string _userName = "Username";
    private string _password = "Password123";

    public UserRegisterDtoBuilder With<TProperty>(Expression<Func<UserRegisterDto, TProperty>> propertySelector, TProperty value)
    {
        if (propertySelector.Body is not MemberExpression member)
        {
            throw new ArgumentException("Expression must be a member expression", nameof(propertySelector));
        }
        var propertyName = member.Member.Name;

        switch (propertyName)
        {
            case nameof(UserRegisterDto.FirstName):
                _firstName = Convert.ToString(value);
                break;
            case nameof(UserRegisterDto.LastName):
                _lastName = Convert.ToString(value);
                break;
            case nameof(UserRegisterDto.Email):
                _email = Convert.ToString(value);
                break;
            case nameof(UserRegisterDto.UserName):
                _userName = Convert.ToString(value);
                break;
            case nameof(UserRegisterDto.Password):
                _password = Convert.ToString(value);
                break;
            default:
                throw new InvalidOperationException($"Property '{propertyName}' is not supported.");
        }
        return this;
    }

    public UserRegisterDto Build()
    {
        return new UserRegisterDto(_firstName, _lastName, _email, _userName, _password);
    }
}


}