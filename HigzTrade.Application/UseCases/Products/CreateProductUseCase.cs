using HigzTrade.Application.DTOs.Requests;
using HigzTrade.Application.DTOs.Responses;
using HigzTrade.Application.Interfaces;
using HigzTrade.Domain.Entities;

namespace HigzTrade.Application.UseCases.Products
{
    public sealed class CreateProductUseCase
    {
        private readonly IProductRepository _productRepository;
        private readonly IAppUnitOfWork _uow;
        private readonly ICategoryQuery _categoryQuery;

        public CreateProductUseCase(
            IProductRepository productRepository,
            IAppUnitOfWork uow,
            ICategoryQuery categoryQuery)
        {
            _productRepository = productRepository;
            _uow = uow;
            _categoryQuery = categoryQuery;
        }

        public async Task<CreateProductResponse> CreateAsync(
            CreateProductRequest request,
            CancellationToken ct)
        {
            Product product = null!;

            if (! await _categoryQuery.CategoryIsExists(request.CategoryId,ct))
            {
                throw new ApplicationException("Invalid Category");
            }

            await _uow.ExecuteAsync(async (token) =>
            {
                // ใช้ 'token' ภายในนี้ หากมีการเรียก Async Methods อื่นๆ
                product = Product.Create(
                    request.Name,
                    request.Sku,
                    request.Price,
                    request.CategoryId);

                _productRepository.Add(product);


                //await _productRepository.AddAsync(product, token);
                await Task.CompletedTask;
                //return Task.CompletedTask;    

            }, ct);
            
            return new CreateProductResponse(product.ProductId);
        }
    }
}
