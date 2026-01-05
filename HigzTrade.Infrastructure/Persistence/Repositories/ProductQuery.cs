//using HigzTrade.Application.Interfaces;
using HigzTrade.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace HigzTrade.Infrastructure.Persistence.Repositories
{
    public class ProductQuery 
    {
        private readonly HigzTradeDbContext _db;
        public ProductQuery(HigzTradeDbContext db) => _db = db;

        public Task<bool> IsSkuExists(string sku, CancellationToken ct)
        {
            return _db.Products.AnyAsync(ent => ent.Sku.ToLower() == sku.ToLower(), ct);
        }
    }
}
