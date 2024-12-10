using System;
using System.Collections.Generic;

namespace JobPlatform.Data;

public partial class Subscription
{
    public long Id { get; set; }

    public long BusinessId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public int Allowance { get; set; }

    public virtual Business? Business { get; set; }
}
