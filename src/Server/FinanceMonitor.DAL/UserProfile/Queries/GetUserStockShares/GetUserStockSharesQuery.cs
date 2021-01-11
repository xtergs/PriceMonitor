using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Models;
using FinanceMonitor.DAL.Repositories.Interfaces;
using MediatR;

namespace FinanceMonitor.DAL.UserProfile.Queries.GetUserStockShares
{
    public class GetUserStockSharesQuery : IRequest<ICollection<UserPrice>>
    {
        public GetUserStockSharesQuery(string symbol, string userId)
        {
            Symbol = symbol;
            UserId = userId;
        }

        public string Symbol { get; init; }
        public string UserId { get; init; }

        public class GetUserStockSharesQueryHandler : IRequestHandler<GetUserStockSharesQuery, ICollection<UserPrice>>
        {
            private readonly IUserRepository _userRepository;

            public GetUserStockSharesQueryHandler(IUserRepository userRepository)
            {
                _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            }

            public Task<ICollection<UserPrice>> Handle(GetUserStockSharesQuery request,
                CancellationToken cancellationToken)
            {
                return _userRepository.GetUserStockShares(request.UserId, request.Symbol);
            }
        }
    }
}