using System.Reflection.Metadata;
using Moq;
using Xunit;
using FluentAssertions;
using CourseEnrollment.Api.Application.Commands.WithdrawUser;
using CourseEnrollment.Domain.Model;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using CourseEnrollment.Api.Application.Commands.Common;

namespace CourseEnrollment.Api.Tests.Application.Commands
{
    public class WithdrawUserCommandHandlerTest
    {
        private Mock<IUserRepository> UserRepository { get; }
        private Mock<ICourseRepository> CourseRepository { get; }
        private WithdrawUserCommandHandler WithdrawUserCommandHandler { get; }

        public WithdrawUserCommandHandlerTest()
        {
            UserRepository = new Mock<IUserRepository>();
            CourseRepository = new Mock<ICourseRepository>();
            WithdrawUserCommandHandler = new WithdrawUserCommandHandler(UserRepository.Object, CourseRepository.Object);
        }

        [Fact]
        public async Task Handle_WhenCourseDoesNotExists_DoNotEnrollAndReportEnrolled()
        {
            var courseIds = new List<Guid>() { Guid.NewGuid() };
            var user = new User(Guid.NewGuid(), "bob.robson@test.pl");
            user.Enroll(new Course(Guid.NewGuid(), "Math"));

            var command = new WithdrawUserCommand() { CourseIds = courseIds, UserId = user.Id };

            UserRepository.Setup(cr => cr.GetByUserIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult(user));
            CourseRepository.Setup(cr => cr.GetByCourseIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult<Course>(null));

            var result = await WithdrawUserCommandHandler.Handle(command, CancellationToken.None);

            UserRepository.Verify(u => u.UpdateAsync(user), Times.Once());
            result.Should().Be(CommandResultStatus.Success);
            user.Courses.Count.Should().Be(1);
        }

        [Fact]
        public async Task Handle_WhenUserDoesNotExists_ReturnErrorThatUserDoesNotExists()
        {
            var course = new Course(Guid.NewGuid(), "Mathematics");
            var courseIds = new List<Guid>() { course.Id };

            CourseRepository.Setup(cr => cr.GetByCourseIdAsync(course.Id)).Returns(Task.FromResult(course));
            UserRepository.Setup(cr => cr.GetByUserIdAsync(It.IsAny<Guid>())).Returns(Task.FromResult<User>(null));

            var command = new WithdrawUserCommand() { CourseIds = courseIds, UserId = Guid.NewGuid() };
            var result = await WithdrawUserCommandHandler.Handle(command, CancellationToken.None);

            result.Should().Be(CommandResultStatus.NotFound);
        }

        [Fact]
        public async Task Handle_WhenUserAndCourseExists_EnrollUserToCourse_ReturnEnrolled()
        {
            var course = new Course(Guid.NewGuid(), "Mathematics");
            var user = new User(Guid.NewGuid(), "bob.robson@test.pl");
            user.Enroll(course);

            var courseIds = new List<Guid>() { course.Id };
            CourseRepository.Setup(cr => cr.GetByCourseIdAsync(course.Id)).Returns(Task.FromResult(course));
            UserRepository.Setup(cr => cr.GetByUserIdAsync(user.Id)).Returns(Task.FromResult(user));

            var command = new WithdrawUserCommand() { CourseIds = courseIds, UserId = user.Id };
            var result = await WithdrawUserCommandHandler.Handle(command, CancellationToken.None);

            result.Should().Be(CommandResultStatus.Success);
            user.Courses.Count.Should().Be(0);

            UserRepository.Verify(u => u.UpdateAsync(user), Times.Once());
        }
    }
}