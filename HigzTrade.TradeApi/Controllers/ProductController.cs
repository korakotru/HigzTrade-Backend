using HigzTrade.Application.DTOs.Products;
using HigzTrade.Application.UseCases.Products;
using HigzTrade.TradeApi.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace HigzTrade.TradeApi.Controllers
{
    [Route("api/[controller]")]
    [EnableRateLimiting(RateLimitPolicy.DefaultRateLimit)]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly CreateProductUseCase _createProduct;
        private readonly UpdatePriceUseCase _updatePrice;

        public ProductController(CreateProductUseCase createProduct,
            UpdatePriceUseCase updatePrice)
        {
            _createProduct = createProduct;
            _updatePrice = updatePrice;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(
            [FromBody] CreateProductDto.Request request,
            CancellationToken ct)
        {
            return Ok(await _createProduct.CreateAsync(request, ct));
        }

        [HttpPut("update-price")]
        public async Task<IActionResult> UpdatePrice(
            [FromBody] UpdatePriceDto.Request request,
            CancellationToken ct)
        {
            return Ok(await _updatePrice.UpdatePriceAsync(request, ct));
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
