using System;
using System.Collections.Generic;

namespace HigzTrade.Domain.Entities;

public partial class Stock
{
    public int ProductId { get; private set; }

    public int AvailableQty { get; private set; }

    public int ReservedQty { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public virtual Product Product { get; private set; } = null!;
}
