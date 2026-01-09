using HigzTrade.Application.Interfaces;
using HigzTrade.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigzTrade.Infrastructure.Persistence.Repositories
{
    public class CategoryQuery : ICategoryQuery
    {
        private readonly HigzTradeDbContext _db;
        public CategoryQuery(HigzTradeDbContext db) => _db = db;

        public Task<bool> IsCategoryExists(int categoryId, CancellationToken ct)
        {
            return _db.Categories.AnyAsync(c => c.CategoryId == categoryId, ct);
        }
    }
}
