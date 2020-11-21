using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CourseEnrollment.Domain.Model;
using CourseEnrollment.Api.Application.Commands.Common;

namespace CourseEnrollment.Api.Application.Commands.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, CommandResultStatus>
    {
        public IUserRepository UserRepository { get; }
        public DeleteUserCommandHandler(IUserRepository userRepository)
        {
            UserRepository = userRepository;
        }

        public async Task<CommandResultStatus> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
        {
            var user = await UserRepository.GetByUserIdAsync(command.Id);
            if (user == null)
            {
                return CommandResultStatus.NotFound;
            }

            await UserRepository.DeleteAsync(user);
            return CommandResultStatus.Deleted;

        }
    }
}