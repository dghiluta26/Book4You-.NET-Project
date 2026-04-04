using System;

namespace Project.Models
{
    public class UnavailableDate
    {
        public int Id { get; set; }

        public int AccommodationId { get; set; }

        public DateTime Date { get; set; }
    }
}
