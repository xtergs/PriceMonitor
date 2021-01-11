using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Repositories.Interfaces;
using MediatR;

namespace FinanceMonitor.DAL.Stocks.Queries.GetSavedStocks
{
    public class GetSavedStocksQuery : IRequest<ICollection<StockListItemDto>>
    {
        public class GetSavedStocksQueryHandler : IRequestHandler<GetSavedStocksQuery, ICollection<StockListItemDto>>

        {
            private readonly IStockRepository _stockRepository;

            public GetSavedStocksQueryHandler(IStockRepository stockRepository)
            {
                _stockRepository = stockRepository ?? throw new ArgumentNullException(nameof(stockRepository));
            }

            public Task<ICollection<StockListItemDto>> Handle(GetSavedStocksQuery request,
                CancellationToken cancellationToken)
            {
                return _stockRepository.GetSavedStocks();
            }
        }
    }
}