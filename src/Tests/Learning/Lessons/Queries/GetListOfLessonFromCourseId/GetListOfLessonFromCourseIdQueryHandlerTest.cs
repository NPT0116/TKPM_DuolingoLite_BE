using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Learning.Lessons.Queries.GetListOfLessonFromCourseId;
using Domain.Entities.Learning.Courses;
using Domain.Entities.Learning.Lessons;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using SharedKernel;
using Xunit;

namespace Tests.Learning.Lessons.Queries.GetListOfLessonFromCourseId
{
    public class GetListOfLessonFromCourseIdQueryHandlerTest
    {
        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly GetListOfLessonFromCourseIdQueryHandler _handler;

        public GetListOfLessonFromCourseIdQueryHandlerTest()
        {
            _courseRepositoryMock = new Mock<ICourseRepository>();
            // SUT (System Under Test)
            _handler = new GetListOfLessonFromCourseIdQueryHandler(_courseRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenCourseNotFound()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var query = new GetListOfLessonFromCourseIdQuery(courseId);

            // Giả lập repository trả về null => không tìm thấy Course
            _courseRepositoryMock
                .Setup(repo => repo.GetCourseById(courseId))
                .ReturnsAsync((Course)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(CourseError.CourseNotFound(courseId));
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WithLessonsArray_WhenCourseExists()
        {
            // Arrange
            var courseId = Guid.NewGuid();

            // Tạo một Course, bạn có thể dùng factory Course.Create(...) 
            // hoặc constructor nếu domain cho phép.
            // Ở đây minh hoạ: 
            var createCourseResult = Course.Create("Test Course", 1, null);
            createCourseResult.IsSuccess.Should().BeTrue("The test course must be valid.");
            var fakeCourse = createCourseResult.Value;

            // Thêm một vài Lesson (nếu domain cho phép AddLesson)
            var lesson1 = new Lesson("Lesson One", xpEarned: 10, order: 1);
            var lesson2 = new Lesson("Lesson Two", xpEarned: 20, order: 2);
            fakeCourse.AddLesson(lesson1);
            fakeCourse.AddLesson(lesson2);

            // Giả lập repository trả về "fakeCourse"
            _courseRepositoryMock
                .Setup(repo => repo.GetCourseById(courseId))
                .ReturnsAsync(fakeCourse);

            var query = new GetListOfLessonFromCourseIdQuery(courseId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var lessonsArray = result.Value;
            lessonsArray.Should().NotBeNull();
            lessonsArray.Should().HaveCount(2);

            // Kiểm tra LessonDto[0]
            lessonsArray[0].Id.Should().Be(lesson1.Id);
            lessonsArray[0].Title.Should().Be("Lesson One");
            lessonsArray[0].Order.Should().Be(1);
            lessonsArray[0].XpEarned.Should().Be(10);
            lessonsArray[0].QuestionCount.Should().Be(0); // no questions in example

            // Kiểm tra LessonDto[1]
            lessonsArray[1].Id.Should().Be(lesson2.Id);
            lessonsArray[1].Title.Should().Be("Lesson Two");
            lessonsArray[1].Order.Should().Be(2);
            lessonsArray[1].XpEarned.Should().Be(20);
        }
    }
}
