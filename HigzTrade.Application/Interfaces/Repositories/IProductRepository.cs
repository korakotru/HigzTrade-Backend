using HigzTrade.Application.DTOs.Products;
using HigzTrade.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigzTrade.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        void Add(Product product);
        void Delete(Product product);
        Task<Product?> GetByIdAsync(int productId);
    }
}
