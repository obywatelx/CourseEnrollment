using System.Collections.Generic;
using MediatR;

namespace CourseEnrollment.Api.Application.Queries.Course
{
    public record AllCourseQuery : IRequest<IList<Domain.Model.Course>>
    {
    }
}
