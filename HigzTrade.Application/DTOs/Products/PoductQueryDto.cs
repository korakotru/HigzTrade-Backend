using HigzTrade.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigzTrade.Application.DTOs.Products
{
    public class PoductQueryDto
    {
        public sealed record Request(string keyword);
        public sealed record Response(
            int ProductId,
            string Name,
            string Sku,
            decimal Price,
            string Status,
            int CategoryId,
            string CategoryName,
            string CreatedBy,
            DateTime CreatedAt,
            string UpdatedBy,
            DateTime? UpdatedAt,
            int StockQty
        );
    }
}