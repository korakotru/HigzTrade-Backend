using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigzTrade.Application.DTOs.Products
{
    public class DeleteProductDto
    {
        public sealed record Request (int ProductId, string DeletedBy);
    }
}

