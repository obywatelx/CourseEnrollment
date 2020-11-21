using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CourseEnrollment.Api.Application.Commands.DeleteCourse;
using CourseEnrollment.Api.Application.Queries.Course;
using CourseEnrollment.Api.DTOs;
using CourseEnrollment.Api.Extensions;
using CourseEnrollment.Api.Application.Commands.Common;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;

namespace CourseEnrollment.Api.Controllers
{
    [ApiController]
    [Route("courses")]
    public class CoursesController : ControllerBase
    {
        public IMediator Mediator { get; }
        public CoursesController(IMediator mediator)
        {
            Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator)); ;
        }

        /// <summary>
        /// Returns a list of Courses
        /// </summary>
        [HttpGet]
        [SwaggerResponse(200, "The course list was retrieved", typeof(IList<CourseDto>))]
        public async Task<IActionResult> FetchCourses()
        {
            var course = await Mediator.Send(new AllCourseQuery());
            var coursesDto = course.Select(c => new CourseDto() { Id = c.Id, Name = c.Name, Enrolled = c.Users.Count }).ToList();
            return Ok(coursesDto);
        }


        /// <summary>
        /// Retrieves a details of a Course
        /// </summary>
        /// <param name="courseId"></param> 
        [HttpGet]
        [ActionName(nameof(GetByCourseIdAsync))]
        [Route("{courseId}")]
        [SwaggerResponse(200, "The course was retrieved", typeof(CourseDto))]
        [SwaggerResponse(404, "The course could not be found")]
        public async Task<IActionResult> GetByCourseIdAsync(Guid courseId)
        {
            var course = await Mediator.Send(new CourseQuery(courseId));
            if (course == null)
            {
                return NotFound();
            }
            return Ok(course.ToCourseDto());
        }



        /// <summary>
        /// Creates a Course
        /// </summary>
        [HttpPost]
        [SwaggerResponse(201, "The course was created", typeof(CourseDto))]
        [SwaggerResponse(422, "The course already exists")]
        [SwaggerResponse(400, "Request badly formatted or contains unsupported properties")]
        public async Task<IActionResult> CreateCourseAsync([FromBody] CourseDto courseDto)
        {
            if (courseDto.Id != Guid.Empty)
            {
                return Problem("Id cannot be used in request", statusCode: (int)HttpStatusCode.BadRequest);
            }

            if (courseDto.Enrolled != 0)
            {
                return Problem("Enrolled cannot be set in request", statusCode: (int)HttpStatusCode.BadRequest);
            }

            var commandResult = await Mediator.Send(courseDto.ToCreateCommand());

            if (commandResult.Status == CommandResultStatus.DuplicatedEntity)
            {
                return Problem(detail: commandResult.Message, statusCode: (int)HttpStatusCode.UnprocessableEntity);
            }

            var course = commandResult.Entity.ToCourseDto();
            return CreatedAtAction(nameof(GetByCourseIdAsync), new { courseId = course.Id }, course);
        }

        /// <summary>
        /// Deletes a Course
        /// </summary>
        /// <param name="courseId"></param>
        [HttpDelete]
        [Route("{courseId}")]
        [SwaggerResponse(404, "The course does not exists")]
        [SwaggerResponse(204)]
        public async Task<IActionResult> DeleteCourseAsync(Guid courseId)
        {
            var commandResult = await Mediator.Send(new DeleteCourseCommand() { Id = courseId });

            if (commandResult == CommandResultStatus.NotFound)
            {
                return NotFound();
            }
            return NoContent();
        }

    }
}