using HigzTrade.Application.DTOs.Products;
using HigzTrade.Application.Services.Products;
using HigzTrade.TradeApi.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using Serilog.Extensions.Hosting;
using System.Text.Json;

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
        private readonly IDiagnosticContext _diagnosticContext;

        public ProductController(CreateProductUseCase createProduct,
            UpdatePriceUseCase updatePrice,
            DeleteProductUseCase deleteProduct,
            GetProductQuery getProductQuery,   
            IDiagnosticContext diagnosticContext)
        {
            _createProduct = createProduct;
            _updatePrice = updatePrice;
            _deleteProduct = deleteProduct;
            _getProduct = getProductQuery;
            _diagnosticContext = diagnosticContext;
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
            _diagnosticContext.Set("RequestBody", JsonSerializer.Serialize(request));
            return Ok(await _updatePrice.UpdatePriceAsync(request, ct));
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(
            [FromBody] int productId,
            CancellationToken ct)
        {
            var request = new DeleteProductDto.Request(productId, "admin");
            await _deleteProduct.DeleteAsync(request, ct);
            return Ok();
        }

        [HttpPost("search-by-keyword")]
        public async Task<IActionResult> SearchByKeyword(
            [FromBody] ProductQueryDto.Request request,
            CancellationToken ct)
        {
            return Ok(await _getProduct.SearchAsync(request, ct));
        }

        [HttpPost("search-by-id")]
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
