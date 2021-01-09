using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceMonitor.Api.Extensions;
using FinanceMonitor.DAL.Dto;
using FinanceMonitor.DAL.Models;
using FinanceMonitor.DAL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceMonitor.Api.Controllers
{
    [Authorize]
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
            var userId = this.UserId();
            price.UserId = userId;
            return await _stockService.AddUserPrice(price);
        }

        [HttpGet("list")]
        public async Task<IReadOnlyCollection<UserStock>> GetStocks()
        {
            var userId = this.UserId();
            return await _stockService.GetUserStocks(userId);
        }

        [HttpGet("{symbol}/shares")]
        public async Task<IReadOnlyCollection<UserPrice>> GetPrices(string symbol)
        {
            var userId = this.UserId();
            return await _stockService.GetUserStockPrices(userId, symbol);
        }
    }
}