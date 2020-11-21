using System;
using MediatR;
using CourseEnrollment.Api.Application.Commands.Common;

namespace CourseEnrollment.Api.Application.Commands.DeleteUser
{
    public record DeleteUserCommand : IRequest<CommandResultStatus>
    {
        public Guid Id { get; set; }
    }
}