using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceMonitor.Api.Extensions;
using FinanceMonitor.DAL.Models;
using FinanceMonitor.DAL.Services.Interfaces;
using FinanceMonitor.DAL.UserProfile.Commands.AddUserShare;
using FinanceMonitor.DAL.UserProfile.Queries.GetUserStocks;
using FinanceMonitor.DAL.UserProfile.Queries.GetUserStockShares;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceMonitor.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserPricingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserPricingController(IUserStockService stockService, IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost]
        public Task<UserPrice> Add(AddUserShareCommand command)
        {
            command.UserId = this.UserId();
            return _mediator.Send(command);
        }

        [HttpGet("list")]
        public Task<ICollection<UserStock>> GetStocks()
        {
            return _mediator.Send(new GetUserStocksQuery(this.UserId()));
        }

        [HttpGet("{symbol}/shares")]
        public Task<ICollection<UserPrice>> GetPrices(string symbol)
        {
            var userId = this.UserId();
            return _mediator.Send(new GetUserStockSharesQuery(symbol, userId));
        }
    }
}