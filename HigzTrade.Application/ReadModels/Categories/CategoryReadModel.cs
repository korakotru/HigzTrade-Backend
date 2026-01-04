using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigzTrade.Application.ReadModels.Categories
{
    public class CategoryReadModel
    {
        readonly HigzTradeDbContext _db;
        public CategoryReadModel(Cate)
        {
            _db = db;
        }
        public async Task<bool> CategoryIsExists(int categoryId, CancellationToken ct)
        {
            return true; // todo:implement
        }
    }


}
