using System;
using System.Threading;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Repositories.Interfaces;
using MediatR;

namespace FinanceMonitor.DAL.Stocks.Commands.UpdateStockStatus
{
    public class UpdateStockStatusCommand : IRequest<Unit>
    {
        public UpdateStockStatusCommand(string symbol, string status)
        {
            Symbol = symbol;
            Status = status;
        }

        public string Symbol { get; init; }
        public string Status { get; init; }
        
        public class UpdateStockStatusCommandHandler : IRequestHandler<UpdateStockStatusCommand, Unit>
        {
            private readonly IStockRepository _stockRepository;

            public UpdateStockStatusCommandHandler(IStockRepository stockRepository)
            {
                _stockRepository = stockRepository ?? throw new ArgumentNullException(nameof(stockRepository));
            }
            
            public async Task<Unit> Handle(UpdateStockStatusCommand request, CancellationToken cancellationToken)
            {
                 await _stockRepository.UpdateStockStatus(request.Symbol, request.Status);
                 
                 return Unit.Value;
            }
        }
    }
}