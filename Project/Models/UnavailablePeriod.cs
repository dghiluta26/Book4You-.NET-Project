using System;

namespace Project.Models
{
    public class UnavailablePeriod
    {
        public int Id { get; set; }

        public int AccommodationId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
