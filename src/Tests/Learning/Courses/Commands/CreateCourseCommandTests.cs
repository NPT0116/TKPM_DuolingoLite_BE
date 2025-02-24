using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Learning.Courses;
using Application.Interface;
using Domain.Entities.Learning.Courses;
using Domain.Repositories;
using FluentAssertions;
using Moq;
namespace Tests.Learning.Courses.Commands
{
    public class CreateCourseCommandHandlerTests
    {
        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly Mock<IApplicationDbContext> _contextMock;
        public CreateCourseCommandHandlerTests()
        {
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _contextMock = new Mock<IApplicationDbContext>();
        }

        [Fact]
        public async Task Handle_Should_ReturnFailureResult_WhenNameIsEmpty()
        {
            var createCourseDto = new CreateCourseDto(string.Empty, It.IsAny<int>());
            var command = new CreateCourseCommand(createCourseDto);
            var handler = new CreateCourseCommandHandler(_courseRepositoryMock.Object, _contextMock.Object);
            var result = await handler.Handle(command, CancellationToken.None);
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(CourseError.NameIsRequired());
        }

        [Fact]
        public async Task Handle_Should_ReturnFailureResult_WhenLevelIsLessThanOne()
        {
            var courseName = "Test Course";
            var createCourseDto = new CreateCourseDto(courseName, 0);
            var command = new CreateCourseCommand(createCourseDto);
            var handler = new CreateCourseCommandHandler(_courseRepositoryMock.Object, _contextMock.Object);
            var result = await handler.Handle(command, CancellationToken.None);
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(CourseError.LevelMustBeGreaterThanZero());
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenCourseIsCreatedSuccessfully()
        {
            // Arrange
            var createCourseDto = new CreateCourseDto("Valid Course", 1);
            var command = new CreateCourseCommand(createCourseDto);
            var handler = new CreateCourseCommandHandler(_courseRepositoryMock.Object, _contextMock.Object);

            _courseRepositoryMock
                .Setup(repo => repo.CreateCourse(It.IsAny<Course>()))
                .Verifiable();

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Name.Should().Be("Valid Course");
            _courseRepositoryMock.Verify(repo => repo.CreateCourse(It.IsAny<Course>()), Times.Once());
            _contextMock.Verify(context => context.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldCallSaveChangesOnce_WhenCourseIsCreatedSuccessfully()
        {
            // Arrange
            var createCourseDto = new CreateCourseDto("Valid Course", 1);
            var command = new CreateCourseCommand(createCourseDto);
            var handler = new CreateCourseCommandHandler(_courseRepositoryMock.Object, _contextMock.Object);

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            _contextMock.Verify(context => context.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldNotCallSaveChanges_WhenCourseCreationFails()
        {
            // Arrange
            var createCourseDto = new CreateCourseDto(string.Empty, 1); // Invalid input
            var command = new CreateCourseCommand(createCourseDto);
            var handler = new CreateCourseCommandHandler(_courseRepositoryMock.Object, _contextMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            _contextMock.Verify(context => context.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }


    }
}