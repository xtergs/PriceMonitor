using System.Threading.Tasks;
using FinanceMonitor.DAL.UserProfile.Commands.AddUserProfile;
using FinanceMonitor.Messages;
using MediatR;
using Rebus.Handlers;

namespace FinanceMonitor.Api.MessageHandlers
{
    public class UserCreatedHandler : IHandleMessages<UserCreated>
    {
        private readonly IMediator _mediator;

        public UserCreatedHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task Handle(UserCreated message)
        {
            return _mediator.Send(new AddUserProfileCommand(message.UserId));
        }
    }
}