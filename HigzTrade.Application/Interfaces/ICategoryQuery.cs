using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigzTrade.Application.Interfaces
{
    public interface ICategoryQuery
    {
        Task<bool> IsCategoryExists(int categoryId, CancellationToken ct);
    }
}
