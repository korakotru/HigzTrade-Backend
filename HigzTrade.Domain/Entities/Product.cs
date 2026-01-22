using HigzTrade.Domain.Exceptions;

namespace HigzTrade.Domain.Entities;
public partial class Product
{
    private Product() { CreatedBy = "admin"; CreatedAt = DateTime.Now; }
    internal static Product Create(
       string name,
       string sku,
       decimal price,
       int categoryId,
       string  createdBy)
    {
        var errors = new List<string>();

        ValidateName(name, errors);
        ValidateSku(sku, errors);
        ValidatePrice(price, errors);

        if (errors.Any())
            throw new BusinessException(errors);

        return new Product
        {
            Name = name,
            Sku = sku,
            Price = price,
            CategoryId = categoryId,
            Status = "active",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void UpdatePrice(decimal price)
    {
        var errors = new List<string>();

        ValidatePrice(price, errors);

        if (errors.Any())
            throw new BusinessException(errors);

        Price = price;
    }

    private static void ValidatePrice(decimal price, List<string> errors)
    {
        if (price <= 0)
            errors.Add("Price must be greater than zero");
    }

    private static void ValidateName(string name, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(name))
            errors.Add("Name is required");
    }

    private static void ValidateSku(string sku, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(sku))
            errors.Add("Sku is required");
    }

}
