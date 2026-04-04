using Microsoft.EntityFrameworkCore;
using Project.Models;

namespace Project.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Accommodation>().Property(a => a.PricePerNight).HasPrecision(10, 2);
            modelBuilder.Entity<Accommodation>().Property(a => a.Rating).HasPrecision(3, 2);
            modelBuilder.Entity<Booking>().Property(b => b.TotalPrice).HasPrecision(12, 2);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Accommodation> Accommodations { get; set; }

        public DbSet<Booking> Bookings { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<AccommodationAmenity> AccommodationAmenities { get; set; }

        public DbSet<AccommodationImage> AccommodationImages { get; set; }
        public DbSet<Project.Models.UnavailableDate> UnavailableDates { get; set; }
        public DbSet<Project.Models.UnavailablePeriod> UnavailablePeriods { get; set; }
    }
}