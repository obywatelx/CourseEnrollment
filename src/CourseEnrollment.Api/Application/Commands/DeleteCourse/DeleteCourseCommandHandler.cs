using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CourseEnrollment.Domain.Model;
using CourseEnrollment.Api.Application.Commands.Common;
using System;

namespace CourseEnrollment.Api.Application.Commands.DeleteCourse
{
    public class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand, CommandResultStatus>
    {
        public ICourseRepository CourseRepository { get; }
        public DeleteCourseCommandHandler(ICourseRepository userRepository)
        {
            CourseRepository = userRepository;
        }

        public async Task<CommandResultStatus> Handle(DeleteCourseCommand command, CancellationToken cancellationToken)
        {
            var course = await CourseRepository.GetByCourseIdAsync(command.Id);
            if (course == null)
            {
                return CommandResultStatus.NotFound;
            }

            await CourseRepository.DeleteAsync(course);
            return CommandResultStatus.Deleted;
        }
    }
}