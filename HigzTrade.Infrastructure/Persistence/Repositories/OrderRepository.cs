using HigzTrade.Application.Interfaces;
using HigzTrade.Infrastructure.Persistence.Context;

namespace HigzTrade.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly HigzTradeDbContext _context;

        public OrderRepository(HigzTradeDbContext context)
        {
            _context = context;
        }

        //public async Task AddAsync(Order Order)
        //{
        //    await _context.Orders.AddAsync(Order);
        //}
        //// ... method อื่นๆ
    }
}
