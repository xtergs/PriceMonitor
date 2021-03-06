using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceMonitor.Api.Extensions;
using FinanceMonitor.DAL.Dto;
using FinanceMonitor.DAL.Repositories.Interfaces;
using FinanceMonitor.DAL.Services.Interfaces;
using FinanceMonitor.DAL.Stocks.Commands.AddStock;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YahooFinanceApi;

namespace FinanceMonitor.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ApiController : ControllerBase
    {
        private readonly IUserStockService _service;
        private readonly IMediator _mediator;

        public ApiController(IUserStockService service, IMediator mediator)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<object> GetStockDetails(string symbol)
        {
            symbol = symbol.ToUpper();
            var result = await Yahoo.Symbols(symbol)
                .Fields(Enum.GetValues<Field>())
                .QueryAsync();

            return result[symbol].Fields;
        }

        [Authorize]
        [HttpGet]
        public async Task FillDb()
        {
            await _service.AddUserPrice(new AddUserPriceDto
            {
                Symbol = "Googl",
                Price = 56,
                Count = 1,
                DateTime = DateTime.UtcNow,
                UserId = this.UserId()
            });
        }

        [HttpGet]
        public async Task ProcessDailyData([FromServices] IManagementRepository repository)
        {
            var lastDate = DateTime.UtcNow.Date.AddDays(-1);
            await repository.ProcessDailyData(lastDate);
        }

        [HttpPost]
        public async Task<ICollection<string>> MassFillIn(string symbols)
        {
            var symbolsList = symbols.Split(',');

            var failedSymbols = new List<string>();
            foreach (var symbol in symbolsList)
            {
                try
                {
                    await _mediator.Send(new AddStockCommand(symbol));
                }
                catch (Exception ex)
                {
                    failedSymbols.Add(symbol);
                }
            }

            return failedSymbols;
        }
    }
}