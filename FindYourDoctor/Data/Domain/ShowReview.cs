using System;
using System.Collections.Generic;

namespace FindYourDoctor.Data.Domain;

public partial class ShowReview
{
    public int? UserId { get; set; }

    public string? Name { get; set; }

    public string? Surname { get; set; }

    public int? DoctorsRating { get; set; }
}
