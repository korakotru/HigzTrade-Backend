using HigzTrade.Application.DTOs.Requests;
using HigzTrade.Application.UseCases.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
