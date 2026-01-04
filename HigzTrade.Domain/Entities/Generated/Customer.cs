using System;
using System.Collections.Generic;

namespace HigzTrade.Domain.Entities;

public partial class Customer
{
    public int CustomerId { get; private set; }

    public string FullName { get; private set; } = null!;

    public string? Email { get; private set; }

    public string? Phone { get; private set; }

    public string Status { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public virtual ICollection<Order> Orders { get; private set; } = new List<Order>();
}
