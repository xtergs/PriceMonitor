using System;
using System.Threading;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Models;
using FinanceMonitor.DAL.Repositories.Interfaces;
using MediatR;

namespace FinanceMonitor.DAL.Stocks.Queries.GetStockDetails
{
    public class GetStockDetailsQuery : IRequest<Stock>
    {
        public GetStockDetailsQuery(string symbol)
        {
            Symbol = symbol;
        }

        public string Symbol { get; init; }

        public class GetStockDetailsQueryHandler : IRequestHandler<GetStockDetailsQuery, Stock?>
        {
            private readonly IStockRepository _stockRepository;

            public GetStockDetailsQueryHandler(IStockRepository stockRepository)
            {
                _stockRepository = stockRepository ?? throw new ArgumentNullException(nameof(stockRepository));
            }

            public Task<Stock?> Handle(GetStockDetailsQuery request, CancellationToken cancellationToken)
            {
                return _stockRepository.GetStock(request.Symbol);
            }
        }
    }
}