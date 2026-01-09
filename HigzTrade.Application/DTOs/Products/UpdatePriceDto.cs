using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigzTrade.Application.DTOs.Products
{
    public class UpdatePriceDto
    {
        public sealed record Request(
            int ProductId,
            decimal Price 
        );
        public sealed record Response(
            int ProductId,
            int CategoryId,
            string Name,
            string Sku,
            decimal Price,
            string Status,
            DateTime? UpdatedAt
        );
    }
}
