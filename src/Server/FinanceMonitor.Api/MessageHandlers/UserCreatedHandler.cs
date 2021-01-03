using System.Threading.Tasks;
using FinanceMonitor.DAL.Models;
using FinanceMonitor.DAL.Repositories;
using FinanceMonitor.DAL.Repositories.Interfaces;
using FinanceMonitor.Messages;

namespace FinanceMonitor.Api.MessageHandlers
{
    public class UserCreatedHandler : Rebus.Handlers.IHandleMessages<UserCreated>
    {
        private readonly IUserRepository _repository;

        public UserCreatedHandler(IUserRepository repository)
        {
            _repository = repository;
        }
            
        public async Task Handle(UserCreated message)
        {
            await _repository.AddUser(new UserProfile()
            {
                Id = message.UserId
            });
        }
    }
}