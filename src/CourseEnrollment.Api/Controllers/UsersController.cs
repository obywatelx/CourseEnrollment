using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CourseEnrollment.Api.Application.Commands.Common;
using CourseEnrollment.Api.Application.Commands.DeleteUser;
using CourseEnrollment.Api.Application.Queries.QueryUser;
using CourseEnrollment.Api.DTOs;
using CourseEnrollment.Api.Extensions;
using CourseEnrollment.Api.Application.Commands.EnrollUser;
using CourseEnrollment.Api.Application.Commands.WithdrawUser;
using Swashbuckle.AspNetCore.Annotations;


namespace CourseEnrollment.Api.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        public IMediator Mediator { get; }
        public UsersController(IMediator mediator)
        {
            Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }


        /// <summary>
        /// Retrieves a details of a User
        /// </summary>
        /// <param name="userId"></param> 
        [HttpGet]
        [ActionName(nameof(GetByUserIdAsync))]
        [Route("{userId}")]
        [SwaggerResponse(200, "The user was retrieved", typeof(UserDto))]
        [SwaggerResponse(404, "The user could not be found")]
        public async Task<IActionResult> GetByUserIdAsync(Guid userId)
        {
            var user = await Mediator.Send(new UserQuery(userId));
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user.ToUserDto());
        }

        /// <summary>
        /// Creates a User
        /// </summary>
        [HttpPost]
        [SwaggerResponse(201, "The user was created", typeof(UserDto))]
        [SwaggerResponse(422, "The user already exists")]
        [SwaggerResponse(400, "Request badly formatted or contains unsupported properties")]
        public async Task<IActionResult> CreateUserAsync([FromBody] UserDto userDto)
        {
            if (userDto.Id != Guid.Empty)
            {
                return Problem("Id cannot be used in request", statusCode: (int)HttpStatusCode.BadRequest);
            }

            var commandResult = await Mediator.Send(userDto.ToCreateCommand());

            if (commandResult.Status == CommandResultStatus.DuplicatedEntity)
            {
                return Problem(detail: commandResult.Message, statusCode: (int)HttpStatusCode.UnprocessableEntity);
            }

            var user = commandResult.Entity.ToUserDto();
            return CreatedAtAction(nameof(GetByUserIdAsync), new
            {
                userId = user.Id
            }, user);
        }

        /// <summary>
        /// Deletes a User
        /// </summary>
        /// <param name="userId"></param>
        [HttpDelete]
        [Route("{userId}")]
        [SwaggerResponse(404, "The user does not exists")]
        [SwaggerResponse(204)]
        public async Task<IActionResult> DeleteUserAsync(Guid userId)
        {
            var commandResult = await Mediator.Send(new DeleteUserCommand() { Id = userId });

            if (commandResult == CommandResultStatus.NotFound)
            {
                return NotFound();
            }
            return NoContent();
        }

        /// <summary>
        /// Enroll a User into Courses
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="courseIds"></param>
        [HttpPost]
        [Route("{userId}/relationship/courses")]
        [SwaggerResponse(404, "The user does not exists")]
        [SwaggerResponse(204)]
        public async Task<IActionResult> EnrollUserToCourseAsync(Guid userId, [FromBody] IList<EnrollWithdrawUserDto> courseIds)
        {
            var commandResult = await Mediator.Send(new EnrollUserCommand() { UserId = userId, CourseIds = courseIds.Select(s => s.Id).ToList() });
            if (commandResult == CommandResultStatus.Success)
            {
                return NoContent();
            }
            return NotFound();
        }

        /// <summary>
        /// Withdraw a User into Courses
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="courseIds"></param>
        [HttpDelete]
        [Route("{userId}/relationship/courses")]
        [SwaggerResponse(404, "The user does not exists")]
        [SwaggerResponse(204)]
        public async Task<IActionResult> WithdrawUserFromCourse(Guid userId, [FromBody] IList<EnrollWithdrawUserDto> courseIds)
        {
            var commandResult = await Mediator.Send(new WithdrawUserCommand() { UserId = userId, CourseIds = courseIds.Select(s => s.Id).ToList() });
            if (commandResult == CommandResultStatus.Success)
            {
                return NoContent();
            }
            return NotFound();
        }

        /// <summary>
        /// Retrieves a list of Courses a User is enrolled into
        /// </summary>
        /// <param name="userId"></param>
        [HttpGet]
        [Route("{userId}/relationship/courses")]
        [SwaggerResponse(200, "List of courses retrieved", typeof(IList<CourseDto>))]
        public async Task<IActionResult> FetchCourses(Guid userId)
        {
            var courses = await Mediator.Send(new CourseEnrollmentQuery { UserId = userId });
            var coursesDto = courses.Select(c => new CourseDto { Id = c.Id, Name = c.Name });
            return Ok(coursesDto);
        }
    }
}