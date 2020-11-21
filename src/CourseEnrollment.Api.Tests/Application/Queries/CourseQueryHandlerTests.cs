using System.Threading.Tasks;
using System.Threading;
using Moq;
using Xunit;
using FluentAssertions;
using CourseEnrollment.Domain.Model;
using CourseEnrollment.Api.Application.Queries.Course;
using System;

namespace CourseEnrollment.Api.Tests.Application.Queries
{
    public class CourseQueryHandlerTests
    {
        private Mock<ICourseRepository> CourseRepository { get; }
        private CourseQueryHandler CourseQueryHandler { get; }
        public CourseQueryHandlerTests()
        {
            CourseRepository = new Mock<ICourseRepository>();
            CourseQueryHandler = new CourseQueryHandler(CourseRepository.Object);
        }

        [Fact]
        public async Task Handle_WhenCourseDoesNotExistsReturnNull()
        {
            var courseQuery = new CourseQuery(Guid.NewGuid());
            CourseRepository.Setup(ur => ur.GetByCourseIdAsync(courseQuery.CourseId)).Returns(Task.FromResult<Course>(null));

            var course = await CourseQueryHandler.Handle(courseQuery, CancellationToken.None);

            course.Should().BeNull();
        }

        [Fact]
        public async Task Handle_WhenCourseExistsReturnCourse()
        {
            var course = new Course(Guid.NewGuid(), "Mathematics");

            CourseRepository.Setup(ur => ur.GetByCourseIdAsync(course.Id)).Returns(Task.FromResult(course));

            var fetchedCourse = await CourseQueryHandler.Handle(new CourseQuery(course.Id), CancellationToken.None);

            fetchedCourse.Should().Be(course);
        }
    }
}