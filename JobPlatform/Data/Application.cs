using System;
using System.Collections.Generic;

namespace JobPlatform.Data;

public partial class Application
{
    public long Id { get; set; }

    public long? VacancyId { get; set; }

    public long? ApplicantId { get; set; }

    public string? ApplicationText { get; set; }

    public bool Checked { get; set; }= false;

    public bool? Approved { get; set; }= false;
    

    public DateOnly? ApplicationDate { get; set; }

    public virtual Applicant? Applicant { get; set; }

    public virtual Vacancy? Vacancy { get; set; }

    
}
