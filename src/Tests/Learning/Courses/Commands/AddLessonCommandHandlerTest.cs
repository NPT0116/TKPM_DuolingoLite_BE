using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Features.Learning.Courses.AddLesson;
using Application.Interface;
using Domain.Entities.Learning.Courses;
using Domain.Entities.Learning.Lessons;
using Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Tests.Learning.Courses.Commands
{
    public class AddLessonCommandHandlerTest
    {
        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly Mock<IApplicationDbContext> _contextMock;
        public AddLessonCommandHandlerTest()
        {
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _contextMock = new Mock<IApplicationDbContext>();
        }
        [Fact]
        public async Task Handle_Should_ReturnFailureResult_WhenTitleIsEmpty()
        {
            var createLessonDto = new CreateLessonDto(string.Empty, It.IsAny<int>(), It.IsAny<int>());
            var command = new AddLessonCommand(It.IsAny<Guid>(), createLessonDto);
            
            var handler = new AddLessonCommandHandler(_contextMock.Object, _courseRepositoryMock.Object);
            var result = await handler.Handle(command, CancellationToken.None);
            
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(LessonError.TitleIsRequired());
        }
        [Fact]
        public async Task Handle_Should_ReturnFailureResult_WhenXpEarnedIsLessThanZero()
        {
            var notNullLessonTitle = "Test Lesson";
            var createLessonDto = new CreateLessonDto(notNullLessonTitle, -1, It.IsAny<int>());
            var command = new AddLessonCommand(It.IsAny<Guid>(), createLessonDto);
            
            var handler = new AddLessonCommandHandler(_contextMock.Object, _courseRepositoryMock.Object);
            var result = await handler.Handle(command, CancellationToken.None);
            
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(LessonError.XpEarnedMustBeGreaterThanZero());
        }
        [Fact]
        public async Task Handle_Should_ReturnFailureResult_WhenOrderIsLessThanZero()
        {
            var notNullLessonTitle = "Test Lesson";
            var positiveXpEarned = 10;
            var createLessonDto = new CreateLessonDto(notNullLessonTitle, positiveXpEarned, -1);
            var command = new AddLessonCommand(It.IsAny<Guid>(), createLessonDto);
            
            var handler = new AddLessonCommandHandler(_contextMock.Object, _courseRepositoryMock.Object);
            var result = await handler.Handle(command, CancellationToken.None);
            
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(LessonError.OrderMustBeGreaterThanZero());
        }
        [Fact]
        public async Task Handle_Should_ReturnFailureResult_WhenCourseNotFound()
        {
            var notNullLessonTitle = "Test Lesson";
            var positiveXpEarned = 10;
            var positiveOrder = 1;
            _courseRepositoryMock
                .Setup(repo => repo.GetCourseById(It.IsAny<Guid>()))
                .ReturnsAsync((Course?)null);
            var createLessonDto = new CreateLessonDto(notNullLessonTitle, positiveXpEarned, positiveOrder);
            var command = new AddLessonCommand(It.IsAny<Guid>(), createLessonDto);
            
            var handler = new AddLessonCommandHandler(_contextMock.Object, _courseRepositoryMock.Object);
            var result = await handler.Handle(command, CancellationToken.None);
            
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(CourseError.CourseNotFound(It.IsAny<Guid>()));
        }

        [Fact]
        public async Task Handle_Should_AddLessonToCourse_WhenInputIsValid()
        {
            // Arrange
            var notNullLessonTitle = "Test Lesson";
            var positiveXpEarned = 10;
            var positiveOrder = 1;
            var mockCourse = Course.Create("Test Course", 1, null).Value;

            _courseRepositoryMock
                .Setup(repo => repo.GetCourseById(It.IsAny<Guid>()))
                .ReturnsAsync(mockCourse);
                
            var createLessonDto = new CreateLessonDto(notNullLessonTitle, positiveXpEarned, positiveOrder);
            var command = new AddLessonCommand(mockCourse.Id, createLessonDto);
            
            var handler = new AddLessonCommandHandler(_contextMock.Object, _courseRepositoryMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            mockCourse.Lessons.Should().ContainSingle(lesson => lesson.Title == notNullLessonTitle);
        }

        // [Fact]
        // public async Task Handle_ShouldReturnFailure_WhenCourseAlreadyContainsLessonWithSameTitle()
        // {
        //     // Arrange
        //     var lessonTitle = "Duplicate Lesson";
        //     var positiveXpEarned = 10;
        //     var positiveOrder = 1;
        //     var existingLesson = Lesson.Create(lessonTitle, positiveXpEarned, positiveOrder).Value;

        //     var mockCourse = Course.Create("Test Course", 1, null).Value;
        //     mockCourse.AddLesson(existingLesson);

        //     _courseRepositoryMock
        //         .Setup(repo => repo.GetCourseById(It.IsAny<Guid>()))
        //         .ReturnsAsync(mockCourse);

        //     var createLessonDto = new CreateLessonDto(lessonTitle, positiveXpEarned, positiveOrder);
        //     var command = new AddLessonCommand(mockCourse.Id, createLessonDto);
        //     var handler = new AddLessonCommandHandler(_contextMock.Object, _courseRepositoryMock.Object);

        //     // Act
        //     var result = await handler.Handle(command, CancellationToken.None);

        //     // Assert
        //     result.IsFailure.Should().BeTrue();
        //     // result.Error.Should().Be(LessonError.LessonTitleAlreadyExists());
        // }

        [Fact]
        public async Task Handle_ShouldNotSaveChanges_WhenLessonCreationFails()
        {
            // Arrange
            var invalidTitle = string.Empty; // Invalid title
            var createLessonDto = new CreateLessonDto(invalidTitle, 10, 1);
            var command = new AddLessonCommand(It.IsAny<Guid>(), createLessonDto);
            var handler = new AddLessonCommandHandler(_contextMock.Object, _courseRepositoryMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            _contextMock.Verify(context => context.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task Handle_ShouldCallSaveChangesOnce_WhenLessonIsAddedSuccessfully()
        {
            // Arrange
            var validTitle = "Valid Lesson";
            var createLessonDto = new CreateLessonDto(validTitle, 10, 1);
            var mockCourse = Course.Create("Test Course", 1, null).Value;

            _courseRepositoryMock
                .Setup(repo => repo.GetCourseById(It.IsAny<Guid>()))
                .ReturnsAsync(mockCourse);

            var command = new AddLessonCommand(mockCourse.Id, createLessonDto);
            var handler = new AddLessonCommandHandler(_contextMock.Object, _courseRepositoryMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _contextMock.Verify(context => context.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}