using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using FluentAssertions;
using MediatR;
using CourseEnrollment.Api.Controllers;
using CourseEnrollment.Api.Application.Commands.CreateCourse;
using CourseEnrollment.Api.Application.Commands.DeleteCourse;
using CourseEnrollment.Api.Application.Queries.Course;
using CourseEnrollment.Api.Application.Commands.Common;
using CourseEnrollment.Api.DTOs;
using CourseEnrollment.Api.Extensions;
using CourseEnrollment.Domain.Model;
using System.Net;

namespace CourseEnrollment.Api.Tests
{
    public class CoursesControllerTests
    {
        private CourseDto CourseDto { get; } = new CourseDto() { Id = Guid.Empty, Name = "Math" };
        private Mock<IMediator> Mediator { get; }
        private CoursesController CoursesController { get; }

        public CoursesControllerTests()
        {
            Mediator = new Mock<IMediator>();
            CoursesController = new CoursesController(Mediator.Object);
        }

        [Fact]
        public async Task GetByCourseId_WhenCourseDoesNotExists_ReturnNotFound()
        {
            var courseQuery = new CourseQuery(Guid.NewGuid());
            Mediator.Setup(m => m.Send(courseQuery, CancellationToken.None))
                .Returns(Task.FromResult<Course>(null));

            var result = await CoursesController.GetByCourseIdAsync(courseQuery.CourseId);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetByCourseId_WhenCourseExists_ReturnCourse()
        {
            var course = new Course(Guid.NewGuid(), "Mathematics");

            var courseQuery = new CourseQuery(course.Id);
            Mediator.Setup(m => m.Send(courseQuery, CancellationToken.None))
                .Returns(Task.FromResult(course));

            var result = await CoursesController.GetByCourseIdAsync(course.Id);
            result.Should().BeOfType<OkObjectResult>();

            var courseDto = (result as OkObjectResult).Value as CourseDto;
            courseDto.Id.Should().Be(course.Id.ToString());
            courseDto.Name.Should().Be(course.Name.ToString());
        }

        [Fact]
        public async Task CreateCourse_WhenCourseDoesNotExists_CourseIsCreated()
        {
            Mediator_SetupSendCreateCourseCommand(CourseDto);

            var result = await CoursesController.CreateCourseAsync(CourseDto);
            result.Should().BeOfType<CreatedAtActionResult>();

            var courseDto = (result as CreatedAtActionResult).Value as CourseDto; ;
            courseDto.Should().Be(CourseDto);
        }

        [Fact]
        public async Task CreateCourse_WhenCourseExists_ReturnUnprocessableEntity()
        {
            Mediator_SetupReturnResult(CommandResultStatus.DuplicatedEntity);

            var result = await CoursesController.CreateCourseAsync(CourseDto);

            result.Should().BeOfType<ObjectResult>();
            (result as ObjectResult).StatusCode.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        }

        [Fact]
        public async Task DeleteCourse_WhenCourseDoesNotExists_ReturnNotFound()
        {
            Mediator.Setup(m => m.Send(It.IsAny<DeleteCourseCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(CommandResultStatus.NotFound));

            var result = await CoursesController.DeleteCourseAsync(Guid.NewGuid());
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteCourse_WhenCourseExisted_ReturnsNoContent()
        {
            Mediator.Setup(m => m.Send(It.IsAny<DeleteCourseCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(CommandResultStatus.Deleted));

            var result = await CoursesController.DeleteCourseAsync(Guid.NewGuid());
            result.Should().BeOfType<NoContentResult>();
        }

        private void Mediator_SetupSendCreateCourseCommand(CourseDto userDto)
        {
            var createUserCommand = userDto.ToCreateCommand();

            var successfullResult = new CommandResult<Course>
            {
                Status = CommandResultStatus.Success,
                Entity = new Course(userDto.Id, userDto.Name)
            };
            Mediator.Setup(m => m.Send(createUserCommand, CancellationToken.None))
                        .Returns(Task.FromResult(successfullResult));
        }

        private void Mediator_SetupReturnResult(CommandResultStatus status)
        {
            Mediator.Setup(m => m.Send(It.IsAny<CreateCourseCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new CommandResult<Course> { Status = status }));
        }
    }

}