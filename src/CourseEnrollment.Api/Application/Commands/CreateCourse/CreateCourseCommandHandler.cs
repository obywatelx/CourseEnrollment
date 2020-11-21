using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CourseEnrollment.Domain.Model;
using CourseEnrollment.Api.Application.Commands.Common;


namespace CourseEnrollment.Api.Application.Commands.CreateCourse
{
    public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, CommandResult<Course>>
    {
        private ICourseRepository CourseRepository { get; }
        public CreateCourseCommandHandler(ICourseRepository courseRepository)
        {
            CourseRepository = courseRepository;
        }

        public async Task<CommandResult<Course>> Handle(CreateCourseCommand command, CancellationToken cancellationToken)
        {
            bool courseExists = await CourseRepository.CourseExistsAsync(command.Name);

            if (courseExists)
            {
                return new CommandResult<Course>
                {
                    Status = CommandResultStatus.DuplicatedEntity,
                    Message = $"Course with name '{command.Name}' already exists."
                };
            }

            var course = new Course(Guid.NewGuid(), command.Name);

            var createdCourse = await CourseRepository.AddAsync(course);

            return new CommandResult<Course> { Status = CommandResultStatus.Success, Entity = createdCourse };
        }
    }
}
