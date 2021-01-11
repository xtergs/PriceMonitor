using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Enums;
using FinanceMonitor.DAL.Models;
using FinanceMonitor.DAL.Repositories.Interfaces;
using MediatR;

namespace FinanceMonitor.DAL.Stocks.Queries.GetStockHistory
{
    public class GetStockHistoryQuery : IRequest<ICollection<PriceHistory>>
    {
        public GetStockHistoryQuery(string symbol, HistoryType type, DateTime start, DateTime end)
        {
            Symbol = symbol;
            Type = type;
            Start = start;
            End = end;
        }

        public string Symbol { get; init; }
        public HistoryType Type { get; init; }
        public DateTime Start { get; init; }
        public DateTime End { get; init; }

        public class GetStockHistoryQueryHandler : IRequestHandler<GetStockHistoryQuery, ICollection<PriceHistory>>
        {
            private readonly IStockRepository _stockRepository;

            public GetStockHistoryQueryHandler(IStockRepository stockRepository)
            {
                _stockRepository = stockRepository ?? throw new ArgumentNullException(nameof(stockRepository));
            }

            public Task<ICollection<PriceHistory>> Handle(GetStockHistoryQuery request,
                CancellationToken cancellationToken)
            {
                var history = _stockRepository.GetStockHistory(request.Symbol, request.Type,
                    request.Start, request.End);
                return history;
            }
        }
    }
}