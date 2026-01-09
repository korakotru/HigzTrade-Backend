using System;
using System.Collections.Generic;

namespace HigzTrade.Domain.Entities;

public partial class Product
{
    public int ProductId { get; private set; }

    public int CategoryId { get; private set; }

    public string Name { get; private set; } = null!;

    public string Sku { get; private set; } = null!;

    public decimal Price { get; private set; }

    public string Status { get; private set; } = null!;
    public string CreatedBy { get; set; }

    public DateTime CreatedAt { get; private set; }
    public string UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; private set; }

    public virtual Category Category { get; private set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();

    public virtual Stock? Stock { get; private set; }
}
