using HigzTrade.Application.DTOs.Requests;
using HigzTrade.Application.DTOs.Responses;
using HigzTrade.Application.Exceptions;
using HigzTrade.Domain.Entities;
//using HigzTrade.Application.Interfaces;
using HigzTrade.Infrastructure.Persistence.Repositories;
using HigzTrade.Infrastructure.Persistence.UnitOfWork;

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

            if (!await _categoryQuery.CategoryIsExists(request.CategoryId, ct))
            {
                //throw new BusinessException("Invalid Category");
                throw new InvalidTimeZoneException();
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
                await Task.CompletedTask; //ไม่จำเป็นต้องมี async/await ถ้า code ใน scope นี้ไม่มี call async method
                //return Task.CompletedTask;

            }, ct);
            
            return new CreateProductResponse(product.ProductId);
        }
    }
}
