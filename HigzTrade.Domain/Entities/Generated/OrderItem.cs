using System;
using System.Collections.Generic;

namespace HigzTrade.Domain.Entities;

public partial class OrderItem
{
    public long OrderItemId { get; private set; }

    public long OrderId { get; private set; }

    public int ProductId { get; private set; }

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal Subtotal { get; private set; }

    public virtual Order Order { get; private set; } = null!;

    public virtual Product Product { get; private set; } = null!;
}
