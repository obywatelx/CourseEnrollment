using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using FluentAssertions;
using MediatR;
using CourseEnrollment.Api.Controllers;
using CourseEnrollment.Api.Application.Commands.Common;
using CourseEnrollment.Api.Application.Commands.CreateUser;
using CourseEnrollment.Api.Application.Commands.DeleteUser;
using CourseEnrollment.Api.Application.Commands.EnrollUser;
using CourseEnrollment.Api.Application.Commands.WithdrawUser;
using CourseEnrollment.Api.Application.Queries.QueryUser;
using CourseEnrollment.Api.DTOs;
using CourseEnrollment.Api.Extensions;
using CourseEnrollment.Domain.Model;


namespace CourseEnrollment.Api.Tests
{
    public class UserControllerTests
    {
        private UserDto UserDto { get; } = new UserDto() { Id = Guid.Empty, Email = "john.doe@wp.pl" };
        private Mock<IMediator> Mediator { get; }
        private UsersController UsersController { get; }

        public UserControllerTests()
        {
            Mediator = new Mock<IMediator>();
            UsersController = new UsersController(Mediator.Object);
        }

        [Fact]
        public async Task GetByUserId_WhenUserDoesNotExists_ReturnNotFound()
        {
            Mediator.Setup(m => m.Send(It.IsAny<UserQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<User>(null));

            var result = await UsersController.GetByUserIdAsync(Guid.NewGuid());
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetByUserId_WhenUserExists_ReturnUser()
        {
            var user = new User(Guid.NewGuid(), "john.doe");

            Mediator.Setup(m => m.Send(new UserQuery(user.Id), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(user));

            var result = await UsersController.GetByUserIdAsync(user.Id);
            result.Should().BeOfType<OkObjectResult>();

            var userDto = (result as OkObjectResult).Value as UserDto;
            userDto.Id.Should().Be(user.Id.ToString());
            userDto.Email.Should().Be(user.Email);
        }

        [Fact]
        public async Task CreateUser_WhenUserDoesNotExists_UserIsCreated()
        {
            Mediator_SetupSendCreateUserCommand(UserDto);

            var result = await UsersController.CreateUserAsync(UserDto);
            result.Should().BeOfType<CreatedAtActionResult>();

            var userDtoResponse = (result as CreatedAtActionResult).Value as UserDto;
            userDtoResponse.Should().Be(UserDto);
        }

        [Fact]
        public async Task CreateUser_WhenUserExists_ReturnUnprocessableEntity()
        {
            Mediator_SetupReturnResult(CommandResultStatus.DuplicatedEntity);

            var result = await UsersController.CreateUserAsync(UserDto);

            result.Should().BeOfType<ObjectResult>();
            (result as ObjectResult).StatusCode.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        }

        [Fact]
        public async Task DeleteUser_WhenUserDoesNotExists_ReturnNotFound()
        {
            Mediator.Setup(m => m.Send(It.IsAny<DeleteUserCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(CommandResultStatus.NotFound));

            var result = await UsersController.DeleteUserAsync(Guid.NewGuid());
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteUser_WhenUserExisted_ReturnsNoContent()
        {
            Mediator.Setup(m => m.Send(It.IsAny<DeleteUserCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(CommandResultStatus.Deleted));

            var result = await UsersController.DeleteUserAsync(Guid.NewGuid());
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task EnrollUser_WhenSucceeds_ReturnsNoContent()
        {
            var command = EnrollUserCommandWithRandomCourses();
            Mediator.Setup(m => m.Send(It.Is<EnrollUserCommand>(c => Match(c, command)), CancellationToken.None))
                .Returns(Task.FromResult(CommandResultStatus.Success));

            var enrollDtoList = command.CourseIds.Select(g => new EnrollWithdrawUserDto() { Id = g }).ToList();
            var result = await UsersController.EnrollUserToCourseAsync(command.UserId, enrollDtoList);
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task EnrollUser_WhenUserDoesNotExists_ReturnsNotFoundResult()
        {
            var command = EnrollUserCommandWithRandomCourses();

            Mediator.Setup(m => m.Send(It.Is<EnrollUserCommand>(c => Match(c, command)), CancellationToken.None))
                .Returns(Task.FromResult(CommandResultStatus.NotFound));

            var enrollDtoList = command.CourseIds.Select(g => new EnrollWithdrawUserDto() { Id = g }).ToList();
            var result = await UsersController.EnrollUserToCourseAsync(command.UserId, enrollDtoList);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task WithdrawUser_WhenSucceeds_ReturnsNoContent()
        {
            var command = WithdrawUserCommandWithRandomCourses();
            Mediator.Setup(m => m.Send(It.Is<WithdrawUserCommand>(c => Match(c, command)), CancellationToken.None))
                .Returns(Task.FromResult(CommandResultStatus.Success));

            var withdrawnDtoList = command.CourseIds.Select(g => new EnrollWithdrawUserDto() { Id = g }).ToList();
            var result = await UsersController.WithdrawUserFromCourse(command.UserId, withdrawnDtoList);
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task WithdrawUser_UserDoesNotExists_ReturnsNotFoundResult()
        {
            var command = WithdrawUserCommandWithRandomCourses();
            Mediator.Setup(m => m.Send(It.Is<WithdrawUserCommand>(c => Match(c, command)), CancellationToken.None))
                .Returns(Task.FromResult(CommandResultStatus.NotFound));

            var withdrawnDtoList = command.CourseIds.Select(g => new EnrollWithdrawUserDto() { Id = g }).ToList();
            var result = await UsersController.WithdrawUserFromCourse(command.UserId, withdrawnDtoList);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task FetchUserCourses_WhenNoCourses_ReturnEmptyArray()
        {
            var query = new CourseEnrollmentQuery() { UserId = Guid.NewGuid() };
            Mediator.Setup(m => m.Send(query, CancellationToken.None)).Returns(Task.FromResult<IList<Course>>(new List<Course>()));
            var fetchQuery = await UsersController.FetchCourses(query.UserId);

            fetchQuery.Should().BeOfType<OkObjectResult>();
            var courses = (fetchQuery as OkObjectResult).Value as List<Course>;
            // courses.Count.Should().Be(0);
        }

        [Fact]
        public async Task FetchUserCourses_WhenCourses_ReturnListOfCourses()
        {
            var query = new CourseEnrollmentQuery() { UserId = Guid.NewGuid() };
            var courses = new List<Course>() { new Course(query.UserId, "math") };
            Mediator.Setup(m => m.Send(query, CancellationToken.None)).Returns(Task.FromResult<IList<Course>>(courses));
            var fetchQuery = await UsersController.FetchCourses(query.UserId);

            fetchQuery.Should().BeOfType<OkObjectResult>();
            var receivedCourses = (fetchQuery as OkObjectResult).Value as List<Course>;
            //receivedCourses.Count.Should().Be(0);
        }

        private static bool Match(EnrollUserCommand received, EnrollUserCommand expected)
        {
            return received.UserId == expected.UserId &&
                received.CourseIds.Intersect(expected.CourseIds).Count() == expected.CourseIds.Count;
        }

        private static bool Match(WithdrawUserCommand received, WithdrawUserCommand expected)
        {
            return received.UserId == expected.UserId &&
                received.CourseIds.Intersect(expected.CourseIds).Count() == expected.CourseIds.Count;
        }
        private static EnrollUserCommand EnrollUserCommandWithRandomCourses()
        {
            return new EnrollUserCommand() { UserId = Guid.NewGuid(), CourseIds = new List<Guid>() { Guid.NewGuid(), Guid.NewGuid() } };
        }

        private static WithdrawUserCommand WithdrawUserCommandWithRandomCourses()
        {
            return new WithdrawUserCommand() { UserId = Guid.NewGuid(), CourseIds = new List<Guid>() { Guid.NewGuid() } };
        }

        private void Mediator_SetupSendCreateUserCommand(UserDto userDto)
        {
            var createUserCommand = userDto.ToCreateCommand();

            var successfullResult = new CommandResult<User>
            {
                Status = CommandResultStatus.Success,
                Entity = new User(userDto.Id, userDto.Email)
            };
            Mediator.Setup(m => m.Send(createUserCommand, CancellationToken.None))
                        .Returns(Task.FromResult(successfullResult));
        }

        private void Mediator_SetupReturnResult(CommandResultStatus status)
        {
            Mediator.Setup(m => m.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new CommandResult<User> { Status = status }));
        }
    }

}