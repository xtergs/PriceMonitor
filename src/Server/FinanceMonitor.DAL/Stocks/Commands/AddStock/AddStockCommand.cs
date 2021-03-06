using System;
using System.Threading;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Exceptions;
using FinanceMonitor.DAL.Models;
using FinanceMonitor.DAL.Repositories.Interfaces;
using FinanceMonitor.DAL.Services.Interfaces;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace FinanceMonitor.DAL.Stocks.Commands.AddStock
{
    public class AddStockCommand : IRequest<Unit>
    {
        public AddStockCommand(string symbol)
        {
            Symbol = symbol;
        }
        private string Symbol { get; }
        
        public class AddStockCommandHandler : IRequestHandler<AddStockCommand, Unit>, IRequestExceptionHandler<AddStockCommand, Unit>
        {
            private readonly IStockRepository _stockRepository;
            private readonly IYahooApiService _apiService;
            private readonly ILogger<AddStockCommand> _logger;

            public AddStockCommandHandler(IStockRepository stockRepository,
                IYahooApiService apiService, ILogger<AddStockCommand> logger )
            {
                _stockRepository = stockRepository ?? throw new ArgumentNullException(nameof(stockRepository));
                _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }
            
            public async Task<Unit> Handle(AddStockCommand request, CancellationToken cancellationToken)
            {
                var apiResult = await _apiService.GetStock(request.Symbol);
                if (apiResult == null) throw new NotFoundException("Symbol is not found");

                var existingStock = await _stockRepository.GetStock(request.Symbol);
                if (existingStock == null)
                {
                    await _stockRepository.CreateStock(new Stock
                    {
                        Symbol = request.Symbol,
                        Market = apiResult.Market,
                        Time = apiResult.Time,
                        Timezone = apiResult.Timezone,
                        LongName = apiResult.LongName,
                        ShortName = apiResult.ShortName,
                        Currency = apiResult.Currency,
                        FinancialCurrency = apiResult.FinancialCurrency,
                        Language = apiResult.Language,
                        QuoteType = apiResult.QuoteType
                    });
                }

                return Unit.Value;    
            }

            public async Task Handle(AddStockCommand request, Exception exception, RequestExceptionHandlerState<Unit> state,
                CancellationToken cancellationToken)
            {
                _logger.LogError("Failed to add {Stock}", request.Symbol);
            }
        }
    }
}