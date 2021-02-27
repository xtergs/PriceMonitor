using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Models;
using FinanceMonitor.DAL.Repositories.Interfaces;
using MediatR;

namespace FinanceMonitor.DAL.UserProfile.Queries.GetUserStocks
{
    public class GetUserStocksQuery : IRequest<ICollection<UserStock>>
    {
        public GetUserStocksQuery(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; }

        public class GetUserStocksQueryHandler : IRequestHandler<GetUserStocksQuery, ICollection<UserStock>>
        {
            private readonly IUserRepository _userRepository;

            public GetUserStocksQueryHandler(IUserRepository userRepository)
            {
                _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            }

            public Task<ICollection<UserStock>> Handle(GetUserStocksQuery request, CancellationToken cancellationToken)
            {
                return _userRepository.GetUserStocks(request.UserId);
            }
        }
    }
}