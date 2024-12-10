using System;
using System.Collections.Generic;

namespace JobPlatform.Data;

public partial class Region
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Vacancy> Vacancies { get; set; } = new List<Vacancy>();
}
