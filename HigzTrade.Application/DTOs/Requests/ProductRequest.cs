using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HigzTrade.Application.DTOs.Requests
{
    public sealed record CreateProductRequest(
        string Name,
        string Sku,
        decimal Price,
        int CategoryId
    );
}
