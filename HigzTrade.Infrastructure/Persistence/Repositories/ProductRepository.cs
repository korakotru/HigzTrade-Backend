//using HigzTrade.Application.Interfaces;
using HigzTrade.Application.Interfaces;
using HigzTrade.Domain.Entities;
using HigzTrade.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HigzTrade.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly HigzTradeDbContext _db;

        public ProductRepository(HigzTradeDbContext db)
        {
            _db = db;
        }

        public void Add(Product product)
        {
            _db.Products.Add(product);
        }

        public void Delete(Product product)
        {
            _db.Products.Remove(product);
        }

        public async Task<Product?> GetByIdAsync(int productId)
        {
            return await _db.Products.Where(x => x.ProductId == productId).SingleOrDefaultAsync();
        }
    }
}
