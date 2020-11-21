using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using CourseEnrollment.Api.Application.Commands.DeleteUser;
using CourseEnrollment.Api.Application.Commands.Common;
using CourseEnrollment.Domain.Model;

namespace CourseEnrollment.Api.Tests.Application.Commands
{
    public class DeleteUserCommandHandlerTest
    {
        private Mock<IUserRepository> UserRepository { get; }
        private DeleteUserCommandHandler DeleteUserCommandHandler { get; }
        public DeleteUserCommandHandlerTest()
        {
            UserRepository = new Mock<IUserRepository>();
            DeleteUserCommandHandler = new DeleteUserCommandHandler(UserRepository.Object);
        }

        [Fact]
        public async Task Handle_WhenUserDoesNotExists_ReturnResultWithNotFound()
        {
            var deleteUserCommand = new DeleteUserCommand { Id = Guid.NewGuid() };

            UserRepository.Setup(ur => ur.GetByUserIdAsync(deleteUserCommand.Id)).Returns(Task.FromResult<User>(null));

            var result = await DeleteUserCommandHandler.Handle(deleteUserCommand, CancellationToken.None);

            result.Should().Be(CommandResultStatus.NotFound);
        }

        [Fact]
        public async Task Handle_WhenUserExisted_ReturnResultWithDeleted()
        {
            var user = new User(Guid.NewGuid(), "john.j@wp.pl");
            UserRepository.Setup(ur => ur.GetByUserIdAsync(user.Id)).Returns(Task.FromResult<User>(user));

            var deleteUserCommand = new DeleteUserCommand { Id = user.Id };
            var result = await DeleteUserCommandHandler.Handle(deleteUserCommand, CancellationToken.None);

            result.Should().Be(CommandResultStatus.Deleted);
        }
    }
}