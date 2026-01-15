using Mapster;
using MapsterMapper;
using HigzTrade.Application.DTOs.Products;
using HigzTrade.Application.Interfaces.Repositories;

namespace HigzTrade.Application.Services.Products
{
    public class GetProductQuery
    {
        private readonly IProductQuery _productQuery;

        public GetProductQuery(IProductQuery productQuery)
        {
            _productQuery = productQuery;
        }
        public async Task<ProductQueryDto.PagedResponse<ProductQueryDto.Response>> SearchAsync(
            ProductQueryDto.Request request,
            CancellationToken ct)
        {
            // ส่ง request (ที่มีทั้ง keyword, pageIndex, pageSize) ไปที่ repository
            return await _productQuery.SearchByKeywordAsync(request, ct);
        }

        public async Task<ProductQueryDto.Response> SearchByIdAsync(int productId, CancellationToken ct)
        {
            return await _productQuery.SearchByIdAsync(productId, ct);
        }
    }
}
