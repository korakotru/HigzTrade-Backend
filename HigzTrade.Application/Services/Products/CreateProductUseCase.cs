using HigzTrade.Application.DTOs.Products;
using HigzTrade.Domain.Entities;
using HigzTrade.Domain.Exceptions;
using HigzTrade.Application.Interfaces;
using HigzTrade.Application.Interfaces.Repositories;
using MapsterMapper;

namespace HigzTrade.Application.Services.Products
{
    public sealed class CreateProductUseCase
    {
        private readonly IProductRepository _productRepository;
        private readonly IAppUnitOfWork _uow;
        private readonly ICategoryQuery _categoryQuery;
        private readonly IProductQuery _productQuery;
        private readonly IMapper _mapper;

        public CreateProductUseCase(
            IProductRepository productRepository,
            IProductQuery productQuery,
            IAppUnitOfWork uow,
            ICategoryQuery categoryQuery,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _uow = uow;
            _categoryQuery = categoryQuery;
            _productQuery = productQuery;
            _mapper = mapper;
        }   

        public async Task<CreateProductDto.Response> CreateAsync(
            CreateProductDto.Request request,
            string createdBy,
            CancellationToken ct)
        {
            var errors = new List<string>();
            Product product = null!;

            try
            {
                if (!await _categoryQuery.IsCategoryExists(request.CategoryId, ct)) errors.Add("Invalid Category");
                if (await _productQuery.IsSkuExists(request.Sku, ct)) errors.Add("SKU already exists.");

                product = Product.Create(request.Name, request.Sku, request.Price, request.CategoryId, createdBy);
            }
            catch (BusinessException ex)
            {
                errors.AddRange(ex.Errors);
            }

            if (errors.Any())
            {
                throw new BusinessException(errors);
            }

            await _uow.ExecuteAsync(async (token) =>
            {
                // ใช้ 'token' ภายในนี้ หากมีการเรียก Async Methods อื่นๆ
                _productRepository.Add(product);
                //await _productRepository.AddAsync(product, token);
                await Task.CompletedTask; //ไม่จำเป็นต้องมี async/await ถ้า code ใน scope นี้ไม่มี call async method
                //return Task.CompletedTask;

            }, ct);

            return _mapper.Map<CreateProductDto.Response>(product);
        }
    }
}
