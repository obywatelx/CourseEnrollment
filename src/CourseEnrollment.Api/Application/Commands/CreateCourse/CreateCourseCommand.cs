using CourseEnrollment.Api.Application.Commands.Common;
using CourseEnrollment.Domain.Model;
using MediatR;

namespace CourseEnrollment.Api.Application.Commands.CreateCourse
{
    public record CreateCourseCommand : IRequest<CommandResult<Course>>
    {
        public string Name { get; set; }
    }
}