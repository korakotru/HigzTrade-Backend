using HigzTrade.Application.DTOs.Requests;
using HigzTrade.Application.DTOs.Responses;
//using HigzTrade.Application.Interfaces;
using HigzTrade.Infrastructure.Persistence.Repositories;
using HigzTrade.Infrastructure.Persistence.UnitOfWork;
using HigzTrade.Domain.Entities;

namespace HigzTrade.Application.UseCases.Products
{
    public sealed class CreateProductUseCase
    {
        private readonly ProductRepository _productRepository;
        private readonly EfUnitOfWork _uow;
        private readonly CategoryQuery _categoryQuery;

        public CreateProductUseCase(
            ProductRepository productRepository,
            EfUnitOfWork uow,
            CategoryQuery categoryQuery)
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

            //if (! await _categoryQuery.CategoryIsExists(request.CategoryId,ct))
            //{
            //    throw new ApplicationException("Invalid Category");
            //}

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
