using HigzTrade.Application.DTOs.Products;
using HigzTrade.Application.Interfaces;
using HigzTrade.Domain.Exceptions; 

namespace HigzTrade.Application.UseCases.Products
{
    public class DeleteProductUseCase
    {
        private IAppUnitOfWork _uow;
        private IProductRepository _productRepository;

        public DeleteProductUseCase(IAppUnitOfWork uow, IProductRepository productRepository)
        {
            _uow = uow;
            _productRepository = productRepository;
        }
        public async Task DeleteAsync(DeleteProductDto.Request request, CancellationToken ct)
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId) ?? throw new BusinessException("Product not exists");

            await _uow.ExecuteAsync((_) =>
            {
                _productRepository.Delete(product);
                return Task.CompletedTask;
            }, ct);
        }
    }
}
