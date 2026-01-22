using HigzTrade.Application.DTOs.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigzTrade.Application.Interfaces.Repositories
{
    public interface IProductQuery
    {
        Task<bool> IsSkuExists(string sku, CancellationToken ct);
        Task<ProductQueryDto.PagedResponse<ProductQueryDto.Response>> SearchByKeywordAsync(
            ProductQueryDto.Request request,
            CancellationToken ct);
        Task<ProductQueryDto.Response> SearchByIdAsync(int productId, CancellationToken ct);
    }
}
