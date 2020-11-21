using System;
using System.Collections.Generic;
using MediatR;

namespace CourseEnrollment.Api.Application.Queries.QueryUser
{
    public record CourseEnrollmentQuery : IRequest<IList<Domain.Model.Course>>
    {
        public Guid UserId { get; set; }
    }
}
