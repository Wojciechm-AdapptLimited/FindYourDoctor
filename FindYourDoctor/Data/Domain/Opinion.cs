using System;
using System.Collections.Generic;

namespace FindYourDoctor.Data.Domain;

public partial class Opinion
{
    public DateTime IssueDate { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public int Stars { get; set; }

    public string? Description { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;
}
