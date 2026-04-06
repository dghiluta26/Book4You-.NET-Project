using System.Collections.Generic;

namespace Project.Models
{
    public class CreateUnavailablePeriodViewModel
    {
        public UnavailablePeriod UnavailablePeriod { get; set; } = new();
        public List<Accommodation> Accommodations { get; set; } = new();
    }
}