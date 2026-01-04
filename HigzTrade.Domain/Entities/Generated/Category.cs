using System;
using System.Collections.Generic;

namespace HigzTrade.Domain.Entities;

public partial class Category
{
    public int CategoryId { get; private set; }

    public string Name { get; private set; } = null!;

    public string? Description { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public virtual ICollection<Product> Products { get; private set; } = new List<Product>();
}
