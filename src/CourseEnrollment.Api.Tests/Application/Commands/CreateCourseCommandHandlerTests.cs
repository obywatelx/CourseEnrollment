using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using CourseEnrollment.Api.Application.Commands.CreateCourse;
using CourseEnrollment.Api.Application.Commands.Common;
using CourseEnrollment.Domain.Model;
using System;

namespace CourseEnrollment.Api.Tests.Application.Commands
{
    public class CreateCourseCommandHandlerTest
    {
        private Mock<ICourseRepository> CourseRepository { get; }
        private CreateCourseCommandHandler CreateCourseCommandHandler { get; }
        public CreateCourseCommandHandlerTest()
        {
            CourseRepository = new Mock<ICourseRepository>();
            CreateCourseCommandHandler = new CreateCourseCommandHandler(CourseRepository.Object);
        }

        [Fact]
        public async Task Handle_WhenCourseAlreadyExists_ReturnCourseAlreadyExists()
        {
            var command = new CreateCourseCommand() { Name = "Physics" };
            CourseRepository.Setup(ur => ur.CourseExistsAsync(command.Name)).Returns(Task.FromResult(true));

            var result = await CreateCourseCommandHandler.Handle(command, CancellationToken.None);

            result.Status.Should().Be(CommandResultStatus.DuplicatedEntity);
        }

        [Fact]
        public async Task Handle_WhenCourseDoesNotExists_CreateCourseAndReturnIt()
        {
            var course = new Course(Guid.NewGuid(), "Physics");

            CourseRepository.Setup(ur => ur.CourseExistsAsync(course.Name)).Returns(Task.FromResult(false));
            CourseRepository.Setup(ur => ur.AddAsync(It.IsAny<Course>())).Returns((Course course) => Task.FromResult(course));

            var command = new CreateCourseCommand() { Name = course.Name };
            var result = await CreateCourseCommandHandler.Handle(command, CancellationToken.None);

            result.Status.Should().Be(CommandResultStatus.Success);
            result.Entity.Id.Should().NotBe(Guid.Empty);
            result.Entity.Name.Should().Be(command.Name);
        }
    }
}