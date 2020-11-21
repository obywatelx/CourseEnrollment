using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CourseEnrollment.Domain.Model;
using CourseEnrollment.Api.Application.Commands.Common;


namespace CourseEnrollment.Api.Application.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CommandResult<User>>
    {
        public IUserRepository UserRepository { get; }
        public CreateUserCommandHandler(IUserRepository userRepository)
        {
            UserRepository = userRepository;
        }

        public async Task<CommandResult<User>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            bool userExists = await UserRepository.UserExistsAsync(command.Email);

            if (userExists)
            {
                return new CommandResult<User>
                {
                    Status = CommandResultStatus.DuplicatedEntity,
                    Message = $"User with email '{command.Email}' already exists."
                };
            }

            var user = new User(Guid.NewGuid(), command.Email);
            var createdUser = await UserRepository.AddAsync(user);
            return new CommandResult<User> { Status = CommandResultStatus.Success, Entity = createdUser };
        }
    }
}