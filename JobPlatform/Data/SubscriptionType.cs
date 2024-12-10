using System;
using System.Collections.Generic;

namespace JobPlatform.Data;

public partial class SubscriptionType
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public decimal Price { get; set; }

    public int DurationDays { get; set; }

    public int Allowance { get; set; }
}
