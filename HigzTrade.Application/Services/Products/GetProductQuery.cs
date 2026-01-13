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
        public async Task<List<PoductQueryDto.Response>> SearchAsync(PoductQueryDto.Request request, CancellationToken ct)
        {
            return await _productQuery.SearchByKeywordAsync(request.keyword, ct);
        }

        public async Task<PoductQueryDto.Response> SearchByIdAsync(int productId, CancellationToken ct)
        {
            return await _productQuery.SearchByIdAsync(productId, ct);
        }
    }
}
