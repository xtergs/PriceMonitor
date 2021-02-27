using System;
using System.Threading;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Dto;
using FinanceMonitor.DAL.Models;
using FinanceMonitor.DAL.Services.Interfaces;
using MediatR;
#pragma warning disable 8618


namespace FinanceMonitor.DAL.UserProfile.Commands.AddUserShare
{
    public class AddUserShareCommand : IRequest<UserPrice>
    {
        public string UserId { get; set; }
        public string Symbol { get; init; }
        public double Price { get; init; }
        public int Count { get; init; }
        public DateTime DateTime { get; init; }

        public class AddUserShareCommandHandler : IRequestHandler<AddUserShareCommand, UserPrice>
        {
            private readonly IUserStockService _stockService;

            public AddUserShareCommandHandler(IUserStockService stockService)
            {
                _stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
            }

            public Task<UserPrice> Handle(AddUserShareCommand request, CancellationToken cancellationToken)
            {
                return _stockService.AddUserPrice(new AddUserPriceDto
                {
                    Count = request.Count,
                    Price = request.Price,
                    Symbol = request.Symbol,
                    DateTime = request.DateTime,
                    UserId = request.UserId
                });
            }
        }
    }
}