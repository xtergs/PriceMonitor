using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Models;
using FinanceMonitor.DAL.Repositories.Interfaces;
using MediatR;

namespace FinanceMonitor.DAL.Stocks.Queries.GetStockDaily
{
    public class GetStockDailyQuery : IRequest<ICollection<PriceDaily>>
    {
        public GetStockDailyQuery(string symbol, DateTime dateTime)
        {
            Symbol = symbol;
            DateTime = dateTime;
        }

        public string Symbol { get; init; }
        public DateTime DateTime { get; init; }

        public class GetStockDailyQueryHandler : IRequestHandler<GetStockDailyQuery, ICollection<PriceDaily>>
        {
            private readonly IStockRepository _stockRepository;

            public GetStockDailyQueryHandler(IStockRepository stockRepository)
            {
                _stockRepository = stockRepository ?? throw new ArgumentNullException(nameof(stockRepository));
            }

            public Task<ICollection<PriceDaily>> Handle(GetStockDailyQuery request, CancellationToken cancellationToken)
            {
                return _stockRepository.GetStockDaily(request.Symbol, request.DateTime);
            }
        }
    }
}