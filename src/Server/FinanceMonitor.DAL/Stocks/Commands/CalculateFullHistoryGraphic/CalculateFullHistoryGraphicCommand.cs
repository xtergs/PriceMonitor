using System;
using System.Threading;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Repositories.Interfaces;
using FinanceMonitor.Messages;
using MediatR;
using Rebus.Bus;

namespace FinanceMonitor.DAL.Stocks.Commands.CalculateFullHistoryGraphic
{
    public class CalculateFullHistoryGraphicCommand : IRequest<Unit>
    {
        public class CalculateFullHistoryGraphicCommandHandler: IRequestHandler<CalculateFullHistoryGraphicCommand, Unit>
        {
            private readonly IManagementRepository _managementRepository;
            private readonly IBus _bus;

            public CalculateFullHistoryGraphicCommandHandler(IManagementRepository managementRepository,
                IBus bus)
            {
                _managementRepository = managementRepository ?? throw new ArgumentNullException(nameof(managementRepository));
                _bus = bus ?? throw new ArgumentNullException(nameof(bus));
            }
            
            public async Task<Unit> Handle(CalculateFullHistoryGraphicCommand request, CancellationToken cancellationToken)
            {
                await _managementRepository.CalculateFullHistoryGraphic();
                await _bus.Send(new SymbolHistoryChanged());
                return Unit.Value;
            }
        }
    }
}