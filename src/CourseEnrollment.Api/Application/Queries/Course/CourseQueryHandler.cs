using System;
using System.Threading;
using System.Threading.Tasks;
using CourseEnrollment.Domain.Model;
using MediatR;

namespace CourseEnrollment.Api.Application.Queries.Course
{
    public class CourseQueryHandler : IRequestHandler<CourseQuery, Domain.Model.Course>
    {
        private ICourseRepository CourseRepository { get; }
        public CourseQueryHandler(ICourseRepository courseRepository)
        {
            CourseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));

        }
        public async Task<Domain.Model.Course> Handle(CourseQuery request, CancellationToken cancellationToken)
        {
            var user = await CourseRepository.GetByCourseIdAsync(request.CourseId);
            return user;
        }
    }
}
