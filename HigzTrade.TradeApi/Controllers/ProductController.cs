using HigzTrade.Application.DTOs.Requests;
using HigzTrade.Application.UseCases.Products;
using HigzTrade.TradeApi.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HigzTrade.TradeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly CreateProductUseCase _createProduct;

        public ProductController(CreateProductUseCase createProduct)
        {
            _createProduct = createProduct;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateProductRequest request,
            CancellationToken ct)
        {
            var result = await _createProduct.CreateAsync(request, ct);
            return Ok(result);
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
