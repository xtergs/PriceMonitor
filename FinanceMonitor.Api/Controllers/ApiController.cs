using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YahooFinanceApi;

namespace FinanceMonitor.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        [HttpGet]
        public async Task<object> GetStockDetails(string symbol)
        {
            symbol = symbol.ToUpper();
            var result = await Yahoo.Symbols(symbol)
                .Fields(Enum.GetValues<Field>())
                .QueryAsync();

            return result[symbol].Fields;
        }
    }
}