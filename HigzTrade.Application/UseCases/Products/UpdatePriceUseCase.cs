using HigzTrade.Application.DTOs.Products;
using HigzTrade.Domain.Entities;
using HigzTrade.Domain.Exceptions;
using HigzTrade.Infrastructure.Persistence.Repositories;
using HigzTrade.Infrastructure.Persistence.UnitOfWork;
using MapsterMapper;

namespace HigzTrade.Application.UseCases.Products
{
    public sealed class UpdatePriceUseCase
    {
        private readonly ProductRepository _productRepository;
        private readonly EfUnitOfWork _uow;
        private readonly CategoryQuery _categoryQuery;
        private readonly ProductQuery _productQuery;
        private readonly IMapper _mapper;

        public UpdatePriceUseCase(
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

        public async Task<UpdatePriceDto.Response> UpdatePriceAsync(
            UpdatePriceDto.Request request,
            CancellationToken ct)
        {
            Product? product = await _productRepository.GetByIdAsync(request.ProductId) ?? throw new BusinessException("Product not exists");
            product.UpdatePrice(request.Price);

            await _uow.ExecuteAsync(null, ct);
            return _mapper.Map<UpdatePriceDto.Response>(product);
        }
    }
}
