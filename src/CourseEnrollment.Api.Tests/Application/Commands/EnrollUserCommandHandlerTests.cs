using System.Reflection.Metadata;
using Moq;
using Xunit;
using FluentAssertions;
using CourseEnrollment.Api.Application.Commands.EnrollUser;
using CourseEnrollment.Domain.Model;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using CourseEnrollment.Api.Application.Commands.Common;

namespace CourseEnrollment.Api.Tests.Application.Commands
{
    public class EnrollUserCommandHandlerTest
    {
        private Mock<IUserRepository> UserRepository { get; }
        private Mock<ICourseRepository> CourseRepository { get; }
        private EnrollUserCommandHandler EnrollUserCommandHandler { get; }

        public EnrollUserCommandHandlerTest()
        {
            UserRepository = new Mock<IUserRepository>();
            CourseRepository = new Mock<ICourseRepository>();
            EnrollUserCommandHandler = new EnrollUserCommandHandler(UserRepository.Object, CourseRepository.Object);
        }

        [Fact]
        public async Task Handle_WhenCourseDoesNotExists_DoNotEnrollAndReportEnrolled()
        {
            var user = new User(Guid.NewGuid(), "bob.robson@test.pl");
            var courseIds = new List<Guid>() { Guid.NewGuid() };
            var command = new EnrollUserCommand() { CourseIds = courseIds, UserId = user.Id };
            UserRepository.Setup(cr => cr.GetByUserIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult<User>(user));
            CourseRepository.Setup(cr => cr.GetByCourseIdAsync(command.CourseIds.First())).Returns(Task.FromResult<Course>(null));

            var result = await EnrollUserCommandHandler.Handle(command, CancellationToken.None);

            UserRepository.Verify(u => u.UpdateAsync(user), Times.Once());
            result.Should().Be(CommandResultStatus.Success);
            user.Courses.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_WhenUserDoesNotExists_ReturnErrorThatUserDoesNotExists()
        {
            var course = new Course(Guid.NewGuid(), "Mathematics");
            var courseIds = new List<Guid>() { course.Id };

            CourseRepository.Setup(cr => cr.GetByCourseIdAsync(course.Id)).Returns(Task.FromResult<Course>(course));
            UserRepository.Setup(cr => cr.GetByUserIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult<User>(null));

            var command = new EnrollUserCommand() { CourseIds = courseIds, UserId = Guid.NewGuid() };
            var result = await EnrollUserCommandHandler.Handle(command, CancellationToken.None);

            result.Should().Be(CommandResultStatus.NotFound);
        }

        [Fact]
        public async Task Handle_WhenUserAndCourseExists_EnrollUserToCourse_ReturnEnrolled()
        {
            var course = new Course(Guid.NewGuid(), "Mathematics");
            var user = new User(Guid.NewGuid(), "bob.robson@test.pl");
            var courseIds = new List<Guid>() { course.Id };

            CourseRepository.Setup(cr => cr.GetByCourseIdAsync(course.Id)).Returns(Task.FromResult<Course>(course));
            UserRepository.Setup(cr => cr.GetByUserIdAsync(user.Id)).Returns(Task.FromResult<User>(user));

            var command = new EnrollUserCommand() { CourseIds = courseIds, UserId = user.Id };
            var result = await EnrollUserCommandHandler.Handle(command, CancellationToken.None);

            result.Should().Be(CommandResultStatus.Success);
            user.Courses.Single().Should().Be(course);

            UserRepository.Verify(u => u.UpdateAsync(user), Times.Once());
        }
    }
}