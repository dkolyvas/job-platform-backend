using System;
using System.Collections.Generic;

namespace JobPlatform.Data;

public partial class VacancyMerit
{
    public long Id { get; set; }

    public long? VacancyId { get; set; }

    public string? Name { get; set; }

    public virtual Vacancy? Vacancy { get; set; }
}
