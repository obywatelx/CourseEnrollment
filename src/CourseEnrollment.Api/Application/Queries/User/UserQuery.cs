using System;
using CourseEnrollment.Domain.Model;
using MediatR;

namespace CourseEnrollment.Api.Application.Queries.QueryUser
{
    public record UserQuery : IRequest<User>
    {
        public Guid UserId { get; private set; }

        public UserQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
