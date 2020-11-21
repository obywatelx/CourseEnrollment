using System;
using System.Collections.Generic;
using CourseEnrollment.Api.Application.Commands.Common;
using MediatR;

namespace CourseEnrollment.Api.Application.Commands.EnrollUser
{
    public record EnrollUserCommand : IRequest<CommandResultStatus>
    {
        public Guid UserId { get; set; }
        public IList<Guid> CourseIds { get; set; }
    }
}