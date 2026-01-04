using HigzTrade.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigzTrade.Application.Interfaces
{
    public interface IProductRepository
    {
        void Add(Product product);
        //Task AddAsync(Product product, CancellationToken ct);
    }
}
