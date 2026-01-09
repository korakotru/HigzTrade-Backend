using HigzTrade.Application.DTOs.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigzTrade.Application.Interfaces
{
    public interface IProductQuery
    {
        Task<bool> IsSkuExists(string sku, CancellationToken ct);
        Task<List<PoductQueryDto.Response>> SearchByKeywordAsync(string keyword, CancellationToken ct);
        Task<PoductQueryDto.Response> SearchByIdAsync(int productId, CancellationToken ct);
    }
}
