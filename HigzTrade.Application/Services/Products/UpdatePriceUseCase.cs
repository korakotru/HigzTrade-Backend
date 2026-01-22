using HigzTrade.Application.DTOs.Products;
using HigzTrade.Domain.Entities;
using HigzTrade.Domain.Exceptions;
using HigzTrade.Application.Interfaces;
using HigzTrade.Application.Interfaces.Repositories;
using MapsterMapper;

namespace HigzTrade.Application.Services.Products
{
    public sealed class UpdatePriceUseCase
    {
        private readonly IProductRepository _productRepository;
        private readonly IAppUnitOfWork _uow;
        private readonly ICategoryQuery _categoryQuery;
        private readonly IProductQuery _productQuery;
        private readonly IMapper _mapper;

        public UpdatePriceUseCase(
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
