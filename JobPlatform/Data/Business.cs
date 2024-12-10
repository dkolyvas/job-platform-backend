using System;
using System.Collections.Generic;

namespace JobPlatform.Data;

public partial class Business
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Website { get; set; }

    public string? Image { get; set; }

    public long? UserId { get; set; }

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    public virtual User? User { get; set; }

    public virtual ICollection<Vacancy> Vacancies { get; set; } = new List<Vacancy>();
}
