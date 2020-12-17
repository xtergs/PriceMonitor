using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Dto;
using FinanceMonitor.DAL.Models;
using FinanceMonitor.DAL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinanceMonitor.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserPricingController : ControllerBase
    {
        private readonly IUserStockService _stockService;

        public UserPricingController(IUserStockService stockService)
        {
            _stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
        }

        [HttpPost]
        public async Task<UserPrice> Add(AddUserPriceDto price)
        {
            return await _stockService.AddUserPrice(price);
        }

        [HttpGet("{stockId:Guid}/list")]
        public async Task<IReadOnlyCollection<UserPrice>> GetPrices(Guid stockId)
        {
            return await _stockService.GetUserStockPrices(Guid.Empty, stockId);
        }
    }
}