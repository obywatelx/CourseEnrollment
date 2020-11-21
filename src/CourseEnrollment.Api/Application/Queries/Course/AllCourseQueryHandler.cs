using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CourseEnrollment.Domain.Model;
using MediatR;

namespace CourseEnrollment.Api.Application.Queries.Course
{
    public class AllCourseQueryHandler : IRequestHandler<AllCourseQuery, IList<Domain.Model.Course>>
    {
        private ICourseRepository CourseRepository { get; }
        public AllCourseQueryHandler(ICourseRepository courseRepository)
        {
            CourseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));

        }
        public async Task<IList<Domain.Model.Course>> Handle(AllCourseQuery request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(CourseRepository.GetAll());
        }
    }
}
