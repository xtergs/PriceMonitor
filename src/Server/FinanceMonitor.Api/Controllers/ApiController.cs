using System;
using System.Threading.Tasks;
using FinanceMonitor.Api.Extensions;
using FinanceMonitor.DAL.Dto;
using FinanceMonitor.DAL.Services.Interfaces;
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

        public ApiController(IUserStockService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
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
            await _service.AddUserPrice(new AddUserPriceDto()
            {
                Symbol = "Googl",
                Price = 56,
                Count = 1,
                DateTime = DateTime.UtcNow,
                UserId = this.UserId()
            });
        }

        [Authorize]
        [HttpGet]
        public async Task<bool> TestAuth()
        {
            return true;
        }
    }
}