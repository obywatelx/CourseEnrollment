using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using CourseEnrollment.Api.Application.Commands.CreateUser;
using CourseEnrollment.Api.Application.Commands.Common;
using CourseEnrollment.Domain.Model;
using System;

namespace CourseEnrollment.Api.Tests.Application.Commands
{
    public class CreateUserCommandHandlerTest
    {
        private Mock<IUserRepository> UserRepository { get; }
        private CreateUserCommandHandler CreateUserCommandHandler { get; }
        public CreateUserCommandHandlerTest()
        {
            UserRepository = new Mock<IUserRepository>();
            CreateUserCommandHandler = new CreateUserCommandHandler(UserRepository.Object);
        }

        [Fact]
        public async Task Handle_WhenUserAlreadyExists_ReturnUserAlreadyExists()
        {
            var createUserCommand = new CreateUserCommand() { Email = "test@gmail.com" };
            UserRepository.Setup(ur => ur.UserExistsAsync(createUserCommand.Email)).Returns(Task.FromResult(true));
            var result = await CreateUserCommandHandler.Handle(createUserCommand, CancellationToken.None);
            result.Status.Should().Be(CommandResultStatus.DuplicatedEntity);
        }

        [Fact]
        public async Task Handle_WhenUserDoesNotExists_CreateUserAndReturnIt()
        {
            var user = new User(Guid.NewGuid(), "joe.black@wp.pl");

            UserRepository.Setup(ur => ur.UserExistsAsync(user.Email)).Returns(Task.FromResult(false));
            UserRepository.Setup(ur => ur.AddAsync(It.IsAny<User>())).Returns((User user) => Task.FromResult(user));

            var command = new CreateUserCommand() { Email = "joe.black@wp.pl" };
            var result = await CreateUserCommandHandler.Handle(command, CancellationToken.None);

            result.Status.Should().Be(CommandResultStatus.Success);
            result.Entity.Email.Should().Be(command.Email);
        }
    }
}