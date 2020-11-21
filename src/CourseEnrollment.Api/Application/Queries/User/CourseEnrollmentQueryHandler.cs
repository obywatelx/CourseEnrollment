using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using CourseEnrollment.Domain.Model;
using MediatR;

namespace CourseEnrollment.Api.Application.Queries.QueryUser
{
    public class CourseEnrollmentQueryHandler : IRequestHandler<CourseEnrollmentQuery, IList<Domain.Model.Course>>
    {
        private IUserRepository UserRepository { get; }
        public CourseEnrollmentQueryHandler(IUserRepository userRepository)
        {
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

        }
        public async Task<IList<Domain.Model.Course>> Handle(CourseEnrollmentQuery request, CancellationToken cancellationToken)
        {
            var user = await UserRepository.GetByUserIdAsync(request.UserId);
            return user?.Courses ?? new List<Domain.Model.Course>();
        }
    }
}
