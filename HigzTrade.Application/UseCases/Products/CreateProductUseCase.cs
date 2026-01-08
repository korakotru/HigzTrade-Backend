using HigzTrade.Application.DTOs.Products;
using HigzTrade.Domain.Entities;
using HigzTrade.Domain.Exceptions;
using HigzTrade.Infrastructure.Persistence.Repositories;
using HigzTrade.Infrastructure.Persistence.UnitOfWork;
using MapsterMapper;


namespace HigzTrade.Application.UseCases.Products
{
    public sealed class CreateProductUseCase
    {
        private readonly ProductRepository _productRepository;
        private readonly EfUnitOfWork _uow;
        private readonly CategoryQuery _categoryQuery;
        private readonly ProductQuery _productQuery;
        private readonly IMapper _mapper;

        public CreateProductUseCase(
            ProductRepository productRepository,
            ProductQuery productQuery,
            EfUnitOfWork uow,
            CategoryQuery categoryQuery,
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
            CancellationToken ct)
        {
            var errors = new List<string>();
            Product product = null!;

            try
            {
                if (!await _categoryQuery.IsCategoryExists(request.CategoryId, ct)) errors.Add("Invalid Category");
                if (await _productQuery.IsSkuExists(request.Sku, ct)) errors.Add("SKU already exists.");

                product = Product.Create(request.Name, request.Sku, request.Price, request.CategoryId);
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
