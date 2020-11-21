using System.Threading.Tasks;
using System.Threading;
using Moq;
using Xunit;
using FluentAssertions;
using CourseEnrollment.Domain.Model;
using CourseEnrollment.Api.Application.Queries.QueryUser;
using System;

namespace CourseEnrollment.Api.Tests.Application.Queries
{
    public class UserQueryHandlerTests
    {
        private Mock<IUserRepository> UserRepository { get; }
        private UserQueryHandler UserQueryHandler { get; }
        public UserQueryHandlerTests()
        {
            UserRepository = new Mock<IUserRepository>();
            UserQueryHandler = new UserQueryHandler(UserRepository.Object);
        }

        [Fact]
        public async Task Handle_WhenUserDoesNotExistsReturnNull()
        {
            var query = new UserQuery(Guid.NewGuid());
            UserRepository.Setup(ur => ur.GetByUserIdAsync(query.UserId)).Returns(Task.FromResult<User>(null));

            var user = await UserQueryHandler.Handle(query, CancellationToken.None);

            user.Should().BeNull();
        }

        [Fact]
        public async Task Handle_WhenUserExistsReturnUser()
        {
            var user = new User(Guid.NewGuid(), "john.doe");

            UserRepository.Setup(ur => ur.GetByUserIdAsync(user.Id)).Returns(Task.FromResult(user));

            var fetchedUser = await UserQueryHandler.Handle(new UserQuery(user.Id), CancellationToken.None);

            fetchedUser.Should().Be(user);
        }
    }
}