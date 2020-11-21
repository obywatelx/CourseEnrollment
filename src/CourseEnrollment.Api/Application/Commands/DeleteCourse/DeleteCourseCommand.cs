using System;
using MediatR;
using CourseEnrollment.Api.Application.Commands.Common;

namespace CourseEnrollment.Api.Application.Commands.DeleteCourse
{
    public record DeleteCourseCommand : IRequest<CommandResultStatus>
    {
        public Guid Id { get; set; }
    }
}