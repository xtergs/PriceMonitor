using System;
using System.Threading;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Repositories.Interfaces;
using MediatR;

namespace FinanceMonitor.DAL.Stocks.Commands.CalculateFullHistoryGraphic
{
    public class CalculateFullHistoryGraphicCommand : IRequest<Unit>
    {
        public class CalculateFullHistoryGraphicCommandHandler: IRequestHandler<CalculateFullHistoryGraphicCommand, Unit>
        {
            private readonly IManagementRepository _managementRepository;

            public CalculateFullHistoryGraphicCommandHandler(IManagementRepository managementRepository)
            {
                _managementRepository = managementRepository ?? throw new ArgumentNullException(nameof(managementRepository));
            }
            
            public async Task<Unit> Handle(CalculateFullHistoryGraphicCommand request, CancellationToken cancellationToken)
            {
                await _managementRepository.CalculateFullHistoryGraphic();
                return Unit.Value;
            }
        }
    }
}