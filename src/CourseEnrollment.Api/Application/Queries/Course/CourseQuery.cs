using System;
using MediatR;

namespace CourseEnrollment.Api.Application.Queries.Course
{
    public record CourseQuery : IRequest<Domain.Model.Course>
    {
        public Guid CourseId { get; private set; }

        public CourseQuery(Guid courseId)
        {
            CourseId = courseId;
        }
    }
}
