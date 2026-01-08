using HigzTrade.Application.DTOs.Products;
using HigzTrade.Domain.Exceptions;
using HigzTrade.Infrastructure.Persistence.Repositories;
using HigzTrade.Infrastructure.Persistence.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigzTrade.Application.UseCases.Products
{
    public class DeleteProductUseCase
    {
        private EfUnitOfWork _uow;
        private ProductRepository _productRepository;

        public DeleteProductUseCase(EfUnitOfWork uow, ProductRepository productRepository)
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
