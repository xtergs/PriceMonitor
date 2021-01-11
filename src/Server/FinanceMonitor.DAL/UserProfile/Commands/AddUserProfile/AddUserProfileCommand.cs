using System;
using System.Threading;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Repositories.Interfaces;
using MediatR;

namespace FinanceMonitor.DAL.UserProfile.Commands.AddUserProfile
{
    public class AddUserProfileCommand : IRequest<Unit>
    {
        public AddUserProfileCommand(string id)
        {
            Id = id;
        }

        public string Id { get; init; }

        public class AddUserProfileCommandHandler : IRequestHandler<AddUserProfileCommand, Unit>
        {
            private readonly IUserRepository _userRepository;

            public AddUserProfileCommandHandler(IUserRepository userRepository)
            {
                _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            }

            public async Task<Unit> Handle(AddUserProfileCommand request, CancellationToken cancellationToken)
            {
                await _userRepository.AddUser(new Models.UserProfile
                {
                    Id = request.Id
                });
                return Unit.Value;
            }
        }
    }
}