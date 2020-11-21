using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using CourseEnrollment.Api.Application.Commands.DeleteCourse;
using CourseEnrollment.Api.Application.Commands.Common;
using CourseEnrollment.Domain.Model;

namespace CourseEnrollment.Api.Tests.Application.Commands
{
    public class DeleteCourseCommandHandlerTest
    {
        private Mock<ICourseRepository> CourseRepository { get; }
        private DeleteCourseCommandHandler DeleteCourseCommandHandler { get; }
        public DeleteCourseCommandHandlerTest()
        {
            CourseRepository = new Mock<ICourseRepository>();
            DeleteCourseCommandHandler = new DeleteCourseCommandHandler(CourseRepository.Object);
        }

        [Fact]
        public async Task Handle_WhenCourseDoesNotExists_ReturnResultWithNotFound()
        {
            var deleteCourseCommand = new DeleteCourseCommand { Id = Guid.NewGuid() };
            CourseRepository.Setup(ur => ur.GetByCourseIdAsync(deleteCourseCommand.Id)).Returns(Task.FromResult<Course>(null));

            var result = await DeleteCourseCommandHandler.Handle(deleteCourseCommand, CancellationToken.None);

            result.Should().Be(CommandResultStatus.NotFound);
        }

        [Fact]
        public async Task Handle_WhenCourseExisted_ReturnResultWithDeleted()
        {
            var course = new Course(Guid.NewGuid(), "Physics");
            CourseRepository.Setup(ur => ur.GetByCourseIdAsync(course.Id)).Returns(Task.FromResult<Course>(course));

            var deleteCourseCommand = new DeleteCourseCommand { Id = course.Id };
            var result = await DeleteCourseCommandHandler.Handle(deleteCourseCommand, CancellationToken.None);

            result.Should().Be(CommandResultStatus.Deleted);
        }
    }
}