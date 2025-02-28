using System;
using Application.Features.Learning.Courses.Queries.GetActiveCourseWithAUser;
using Domain.Entities.Learning.Courses;
using Domain.Entities.Learning.LearningProgresses;
using Domain.Repositories;
using FluentAssertions;
using Moq;

namespace Tests.Learning.Courses.Queries.GetActiveCourseWithAUser;

public class GetActiveCourseWithAUserQueryHandlerTest
{
    private readonly Mock<ILearningProgressRepository> _learningProgressRepositoryMock;

    public GetActiveCourseWithAUserQueryHandlerTest()
    {
        _learningProgressRepositoryMock = new Mock<ILearningProgressRepository>();
    }
[Fact]
public async Task Handle_Should_Return_Failure_When_LearningProgress_NotFound()
{
    // Arrange
    var userId = Guid.NewGuid();

    // Giả lập repository trả về null
    _learningProgressRepositoryMock
        .Setup(repo => repo.GetLearningProgressByUserIdAsync(userId))
        .ReturnsAsync((LearningProgress)null);

    var query = new GetActiveCourseWithAUserQuery(userId);
    var handler = new GetActiveCourseWithAUserQueryHandler(_learningProgressRepositoryMock.Object);

    // Act
    var result = await handler.Handle(query, CancellationToken.None);

    // Assert
    result.IsFailure.Should().BeTrue();
    result.Error.Should().Be(LearningProgressError.LearningProgresssForUserNotFound(userId));
}
[Fact]
public async Task Handle_Should_Return_Success_With_CorrectDto_When_LearningProgress_Exists()
{
    // Arrange
    var userId = Guid.NewGuid();
    // var courseId = Guid.NewGuid();
    var level = 3;

    // 1) Tạo Course mock
    var createCourseResult = Course.Create("TestCourse", level, null);
    var fakeCourse = createCourseResult.Value; 
    // Nếu bạn cần gán ID cứng để đối chiếu:
    // Giả sử bạn cho phép SetId (nếu Entity có sẵn hàm SetId) hoặc 
    //   dùng reflection nếu cần:
    // fakeCourse.GetType().BaseType.GetProperty("Id", BindingFlags.Instance | BindingFlags.NonPublic)
    //          ?.SetValue(fakeCourse, courseId);

    // 2) Tạo LearningProgress
    //   (nếu domain chuẩn có factory "LearningProgress.Create")
    var createProgressResult = LearningProgress.Create(userId, fakeCourse);
    var fakeLearningProgress = createProgressResult.Value;

    // Ở domain gốc, LessonOrder mặc định = 1. Nếu bạn muốn cài = 3:
    //   (nếu domain cho phép SetLessonOrder, hoặc bạn "hack" reflection)
    //   fakeLearningProgress.GetType()
    //       .GetProperty("LessonOrder", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
    //       ?.SetValue(fakeLearningProgress, lessonOrder);

    // Mock repository
    _learningProgressRepositoryMock
        .Setup(repo => repo.GetLearningProgressByUserIdAsync(userId))
        .ReturnsAsync(fakeLearningProgress);

    var query = new GetActiveCourseWithAUserQuery(userId);
    var handler = new GetActiveCourseWithAUserQueryHandler(_learningProgressRepositoryMock.Object);

    // Act
    var result = await handler.Handle(query, CancellationToken.None);

    // Assert
    result.IsSuccess.Should().BeTrue();

    var dto = result.Value;
    dto.Should().NotBeNull();
    // dto.CourseId.Should().Be(courseId);
    dto.LessonOrder.Should().Be(1);
    dto.UserId.Should().Be(userId);
}


}
