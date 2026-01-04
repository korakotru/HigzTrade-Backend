using System;
using System.Collections.Generic;

namespace HigzTrade.Domain.Entities;

public partial class Order
{
    public long OrderId { get; private set; }

    public string OrderNumber { get; private set; } = null!;

    public int CustomerId { get; private set; }

    public string Status { get; private set; } = null!;

    public decimal TotalAmount { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public virtual Customer Customer { get; private set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();
}
