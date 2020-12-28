using System.Threading.Tasks;
using FinanceMonitor.DAL.Repositories.Interfaces;
using FinanceMonitor.Messages;

namespace FinanceMonitor.Api.MessageHandlers
{
    public class UserCreatedHandler : Rebus.Handlers.IHandleMessages<UserCreated>
    {
        private readonly IStockRepository _repository;

        public UserCreatedHandler(IStockRepository repository)
        {
            _repository = repository;
        }
            
        public Task Handle(UserCreated message)
        {
            return Task.CompletedTask;
        }
    }
}