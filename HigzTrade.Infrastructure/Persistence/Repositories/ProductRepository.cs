//using HigzTrade.Application.Interfaces;
using HigzTrade.Domain.Entities;
using HigzTrade.Infrastructure.Persistence.Context;

namespace HigzTrade.Infrastructure.Persistence.Repositories
{
    public class ProductRepository //: IProductRepository
    {
        //private readonly HigzTradeDbContext _context;

        //public ProductRepository(HigzTradeDbContext context)
        //{
        //    _context = context;
        //}

        //public async Task AddAsync(Product product)
        //{
        //    await _context.Products.AddAsync(product);
        //}
        //// ... method อื่นๆ
        ///

        private readonly HigzTradeDbContext _db;

        public ProductRepository(HigzTradeDbContext db)
        {
            _db = db;
        }

        public void Add(Product product)
        {
            _db.Products.Add(product);
        }
        //public async Task AddAsync(Product product, CancellationToken ct)
        //{
        //    await _db.Products.AddAsync(product, ct);
        //}
    }
}
