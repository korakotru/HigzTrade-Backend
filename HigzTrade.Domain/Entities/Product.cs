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
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name is required");

        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainException("Sku is required");

        if (price <= 0)
            throw new DomainException("Price must be greater than zero");

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
