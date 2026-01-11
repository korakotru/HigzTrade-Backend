using HigzTrade.Application.DTOs.Products;
using HigzTrade.Application.UseCases.Products;
using HigzTrade.TradeApi.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HigzTrade.TradeApi.Controllers
{
    [Route("api/[controller]")]
    [EnableRateLimiting(RateLimitPolicy.DefaultRateLimit)]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly CreateProductUseCase _createProduct;
        private readonly UpdatePriceUseCase _updatePrice;
        private readonly DeleteProductUseCase _deleteProduct;
        private readonly GetProductQuery _getProduct;

        public ProductController(CreateProductUseCase createProduct,
            UpdatePriceUseCase updatePrice,
            DeleteProductUseCase deleteProduct,
            GetProductQuery getProductQuery)
        {
            _createProduct = createProduct;
            _updatePrice = updatePrice;
            _deleteProduct = deleteProduct;
            _getProduct = getProductQuery;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(
            [FromBody] CreateProductDto.Request request,
            CancellationToken ct)
        {
            string createdBy = "admin";
            return Ok(await _createProduct.CreateAsync(request, createdBy, ct));
        }

        [HttpPut("update-price")]
        public async Task<IActionResult> UpdatePrice(
            [FromBody] UpdatePriceDto.Request request,
            CancellationToken ct)
        {
            return Ok(await _updatePrice.UpdatePriceAsync(request, ct));
        }
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(
            [FromBody] int productId,
            CancellationToken ct)
        {
            var request = new DeleteProductDto.Request(productId, "admin");
            await _deleteProduct.DeleteAsync(request, ct);

            return Ok();
        }

        [HttpGet("search-keyword")]
        public async Task<IActionResult> SearchByKeyword(
            [FromBody] PoductQueryDto.Request request,
            CancellationToken ct)
        {
            return Ok(await _getProduct.SearchAsync(request, ct));
        }

        [HttpGet("search-id")]
        public async Task<IActionResult> SearchById(
            [FromBody] int productId,
            CancellationToken ct)
        {
            return Ok(await _getProduct.SearchByIdAsync(productId, ct));
        }

        //[HttpGet]
        //[RequestTimeout(RequestTimeoutCustomPolicy.BigDataProcessingPolicy)]
        //public async Task<IActionResult> LongtimeProcessing(CancellationToken ct)
        //{
        //    // example for use diffence timeout-policy 

        //    await Task.Delay(30000, ct);
        //    return Ok();
        //}
    }
}
