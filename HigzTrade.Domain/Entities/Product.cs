using HigzTrade.Domain.Exceptions;
using System;
using System.Collections.Generic;

namespace HigzTrade.Domain.Entities;
public partial class Product
{
    public static Product Create(
       string name,
       string sku,
       decimal price,
       int categoryId)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(name)) errors.Add("Name is required");
        if (string.IsNullOrWhiteSpace(sku)) errors.Add("Sku is required");
        if (price <= 0) errors.Add("Price must be greater than zero");

        if (errors.Any()) throw new BusinessException(errors);

        return new Product
        {
            Name = name,
            Sku = sku,
            Price = price,
            CategoryId = categoryId,
            Status = "active",
            CreatedAt = DateTime.UtcNow
        };
    }
}
