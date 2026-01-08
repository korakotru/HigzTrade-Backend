using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigzTrade.Application.DTOs.Products
{
    public class CreateProductDto
    {
        public record Request(
            string Name,
            string Sku,
            decimal Price,
            int CategoryId,
            string CreatedBy
        );
        public sealed record Response(
            int ProductId,
            int CategoryId,
            string Name,
            string Sku,
            decimal Price,
            string Status,
            DateTime CreatedAt
        );

    }
}
