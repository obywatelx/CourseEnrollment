using System;
using System.Threading;
using System.Threading.Tasks;
using CourseEnrollment.Domain.Model;
using MediatR;

namespace CourseEnrollment.Api.Application.Queries.QueryUser
{
    public class UserQueryHandler : IRequestHandler<UserQuery, User>
    {
        private IUserRepository UserRepository { get; }
        public UserQueryHandler(IUserRepository userRepository)
        {
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

        }
        public async Task<User> Handle(UserQuery request, CancellationToken cancellationToken)
        {
            var user = await UserRepository.GetByUserIdAsync(request.UserId);
            return user;
        }
    }
}
