using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HigzTrade.Application.Interfaces;
using HigzTrade.Domain.Entities;

namespace HigzTrade.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        public Task<List<Category>> GetCategoriesAsync(CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
