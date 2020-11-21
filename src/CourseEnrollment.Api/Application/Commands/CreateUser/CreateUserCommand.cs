using MediatR;
using CourseEnrollment.Api.Application.Commands.Common;
using CourseEnrollment.Domain.Model;

namespace CourseEnrollment.Api.Application.Commands.CreateUser
{
    public record CreateUserCommand : IRequest<CommandResult<User>>
    {
        public string Email { get; set; }
    }
}