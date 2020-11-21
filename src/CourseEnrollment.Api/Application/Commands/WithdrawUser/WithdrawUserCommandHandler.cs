using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CourseEnrollment.Domain.Model;
using CourseEnrollment.Api.Application.Commands.Common;

namespace CourseEnrollment.Api.Application.Commands.WithdrawUser
{
    public class WithdrawUserCommandHandler : IRequestHandler<WithdrawUserCommand, CommandResultStatus>
    {
        public IUserRepository UserRepository { get; }
        public ICourseRepository CourseRepository { get; }
        public WithdrawUserCommandHandler(IUserRepository userRepository, ICourseRepository courseRepository)
        {
            UserRepository = userRepository;
            CourseRepository = courseRepository;
        }

        public async Task<CommandResultStatus> Handle(WithdrawUserCommand command, CancellationToken cancellationToken)
        {
            var user = await UserRepository.GetByUserIdAsync(command.UserId);
            if (user == null)
            {
                return CommandResultStatus.NotFound;
            };

            foreach (var courseId in command.CourseIds)
            {
                var course = await CourseRepository.GetByCourseIdAsync(courseId);
                if (course != null)
                {
                    user.Withdrawn(course);
                }
            }
            await UserRepository.UpdateAsync(user);
            return CommandResultStatus.Success;
        }
    }
}