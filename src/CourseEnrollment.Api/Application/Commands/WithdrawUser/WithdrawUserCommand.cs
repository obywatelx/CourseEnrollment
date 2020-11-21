using CourseEnrollment.Api.Application.Commands.Common;
using MediatR;
using System;
using System.Collections.Generic;

namespace CourseEnrollment.Api.Application.Commands.WithdrawUser
{
    public record WithdrawUserCommand : IRequest<CommandResultStatus>
    {
        public Guid UserId { get; set; }
        public IList<Guid> CourseIds { get; set; }
    }
}