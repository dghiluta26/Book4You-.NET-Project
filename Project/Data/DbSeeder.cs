using Project.Models;

namespace Project.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            context.Database.EnsureCreated();

           
            if (!context.Amenities.Any())
            {
                var amenities = new List<Amenity>
                {
                    new Amenity { Name = "WiFi" },
                    new Amenity { Name = "Pool" },
                    new Amenity { Name = "Parking" },
                    new Amenity { Name = "Air Conditioning" },
                    new Amenity { Name = "Breakfast included" },
                    new Amenity { Name = "Kitchen" },
                    new Amenity { Name = "TV" },
                    new Amenity { Name = "Pet friendly" }
                };

                context.Amenities.AddRange(amenities);
                context.SaveChanges();
            }

            // ensure at least one admin user exists
            if (!context.Users.Any(u => u.Role == "Admin"))
            {
                var admin = new User
                {
                    Email = "admin@book4you.local",
                    FirstName = "Admin",
                    LastName = "User",
                    Address = "",
                    Role = "Admin"
                };
                var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
                admin.Password = hasher.HashPassword(admin, "Admin123!");
                context.Users.Add(admin);
                context.SaveChanges();
            }

            // seed some unavailable periods for demonstration (optional)
            if (!context.Set<Project.Models.UnavailablePeriod>().Any())
            {
                var firstAccommodation = context.Accommodations.FirstOrDefault();
                if (firstAccommodation != null)
                {
                    var periods = new List<Project.Models.UnavailablePeriod>
                    {
                        new Project.Models.UnavailablePeriod { AccommodationId = firstAccommodation.Id, StartDate = DateTime.Today.AddDays(3), EndDate = DateTime.Today.AddDays(5) }
                    };

                    context.Set<Project.Models.UnavailablePeriod>().AddRange(periods);
                    context.SaveChanges();
                }
            }

            
            if (!context.Accommodations.Any())
            {
                var accommodations = new List<Accommodation>
                {
                    new Accommodation
                    {
                        Title = "Santorini Cliffside Villa",
                        Location = "Santorini, Greece",
                        Address = "Cliffside Road 12",
                        Description = "Luxury villa overlooking the Aegean Sea with breathtaking views.",
                        PricePerNight = 320,
                        Capacity = 4,
                        Bedrooms = 2,
                        Bathrooms = 2,
                        Type = "Villa",
                        ImageUrl = "/images/stays/villa1.jpg",
                        Rating = 4.9m,
                        IsAvailable = true,
                        CreatedAt = DateTime.Now
                    },
                    new Accommodation
                    {
                        Title = "Santorini Sunset Villa",
                        Location = "Santorini, Greece",
                        Address = "Oia Cliff 7",
                        Description = "Luxury villa with panoramic views over the Aegean Sea and iconic sunsets.",
                        PricePerNight = 350,
                        Capacity = 4,
                        Bedrooms = 2,
                        Bathrooms = 2,
                        Type = "Villa",
                        ImageUrl = "/images/stays/villa2.jpg",
                        Rating = 4.9m,
                        IsAvailable = true,
                        CreatedAt = DateTime.Now
                    },
                    new Accommodation
                    {
                        Title = "Lake Como Luxury Apartment",
                        Location = "Como, Italy",
                        Address = "Via Roma 12",
                        Description = "Modern apartment overlooking Lake Como with a private balcony.",
                        PricePerNight = 280,
                        Capacity = 3,
                        Bedrooms = 1,
                        Bathrooms = 1,
                        Type = "Apartment",
                        ImageUrl = "/images/stays/apartment2.jpg",
                        Rating = 4.8m,
                        IsAvailable = true,
                        CreatedAt = DateTime.Now
                    },
                    new Accommodation
                    {
                        Title = "Paris Boutique Studio",
                        Location = "Paris, France",
                        Address = "Rue de Rivoli 45",
                        Description = "Charming studio in central Paris, close to museums and cafes.",
                        PricePerNight = 190,
                        Capacity = 2,
                        Bedrooms = 1,
                        Bathrooms = 1,
                        Type = "Studio",
                        ImageUrl = "/images/stays/studio2.jpg",
                        Rating = 4.6m,
                        IsAvailable = true,
                        CreatedAt = DateTime.Now
                    },
                    new Accommodation
                    {
                        Title = "Alpine Mountain Cabin",
                        Location = "Zermatt, Switzerland",
                        Address = "Alpine Road 3",
                        Description = "Wooden cabin with direct view of the Matterhorn and ski access.",
                        PricePerNight = 220,
                        Capacity = 4,
                        Bedrooms = 2,
                        Bathrooms = 1,
                        Type = "Cabin",
                        ImageUrl = "/images/stays/cabin2.jpg",
                        Rating = 4.9m,
                        IsAvailable = true,
                        CreatedAt = DateTime.Now
                    },
                    new Accommodation
                    {
                        Title = "Barcelona Beach Apartment",
                        Location = "Barcelona, Spain",
                        Address = "La Rambla 88",
                        Description = "Bright apartment near the beach with modern design.",
                        PricePerNight = 210,
                        Capacity = 3,
                        Bedrooms = 2,
                        Bathrooms = 1,
                        Type = "Apartment",
                        ImageUrl = "/images/stays/apartment3.jpg",
                        Rating = 4.5m,
                        IsAvailable = true,
                        CreatedAt = DateTime.Now
                    },
                    new Accommodation
                    {
                        Title = "Berlin City Loft",
                        Location = "Berlin, Germany",
                        Address = "Alexanderplatz 10",
                        Description = "Spacious loft in the heart of Berlin nightlife.",
                        PricePerNight = 170,
                        Capacity = 2,
                        Bedrooms = 1,
                        Bathrooms = 1,
                        Type = "Apartment",
                        ImageUrl = "/images/stays/apartment4.jpg",
                        Rating = 4.4m,
                        IsAvailable = true,
                        CreatedAt = DateTime.Now
                    },
                    new Accommodation
                    {
                        Title = "Carpathian Forest Cabin",
                        Location = "Sinaia, Romania",
                        Address = "Forest Lane 22",
                        Description = "Quiet cabin surrounded by forests, ideal for relaxation.",
                        PricePerNight = 130,
                        Capacity = 4,
                        Bedrooms = 2,
                        Bathrooms = 1,
                        Type = "Cabin",
                        ImageUrl = "/images/stays/cabin3.jpg",
                        Rating = 4.7m,
                        IsAvailable = true,
                        CreatedAt = DateTime.Now
                    },
                    new Accommodation
                    {
                        Title = "London Modern Flat",
                        Location = "London, UK",
                        Address = "Baker Street 221B",
                        Description = "Stylish flat in central London with easy access to transport.",
                        PricePerNight = 260,
                        Capacity = 2,
                        Bedrooms = 1,
                        Bathrooms = 1,
                        Type = "Apartment",
                        ImageUrl = "/images/stays/apartment5.jpg",
                        Rating = 4.6m,
                        IsAvailable = true,
                        CreatedAt = DateTime.Now
                    },
                    new Accommodation
                    {
                        Title = "Dubai Marina Luxury Suite",
                        Location = "Dubai, UAE",
                        Address = "Marina Walk 5",
                        Description = "Luxury suite with skyline and marina views.",
                        PricePerNight = 420,
                        Capacity = 2,
                        Bedrooms = 1,
                        Bathrooms = 1,
                        Type = "Studio",
                        ImageUrl = "/images/stays/studio3.jpg",
                        Rating = 4.9m,
                        IsAvailable = true,
                        CreatedAt = DateTime.Now
                    },
                    new Accommodation
                    {
                        Title = "Phuket Beach Villa",
                        Location = "Phuket, Thailand",
                        Address = "Beach Road 9",
                        Description = "Tropical villa with private pool and direct beach access.",
                        PricePerNight = 390,
                        Capacity = 5,
                        Bedrooms = 3,
                        Bathrooms = 2,
                        Type = "Villa",
                        ImageUrl = "/images/stays/villa3.jpg",
                        Rating = 4.8m,
                        IsAvailable = true,
                        CreatedAt = DateTime.Now
                    },
                    new Accommodation
                    {
                        Title = "Tokyo Minimal Apartment",
                        Location = "Tokyo, Japan",
                        Address = "Shibuya 11",
                        Description = "Compact and modern apartment in vibrant Shibuya.",
                        PricePerNight = 180,
                        Capacity = 2,
                        Bedrooms = 1,
                        Bathrooms = 1,
                        Type = "Apartment",
                        ImageUrl = "/images/stays/apartment6.jpg",
                        Rating = 4.5m,
                        IsAvailable = true,
                        CreatedAt = DateTime.Now
                    },
                    new Accommodation
                    {
                        Title = "New York Skyline Studio",
                        Location = "New York, USA",
                        Address = "Manhattan 5th Ave",
                        Description = "Studio with amazing skyline views in Manhattan.",
                        PricePerNight = 320,
                        Capacity = 2,
                        Bedrooms = 1,
                        Bathrooms = 1,
                        Type = "Studio",
                        ImageUrl = "/images/stays/studio4.jpg",
                        Rating = 4.7m,
                        IsAvailable = true,
                        CreatedAt = DateTime.Now
                    },
                    new Accommodation
                    {
                        Title = "Toronto Downtown Condo",
                        Location = "Toronto, Canada",
                        Address = "King Street 99",
                        Description = "Modern condo in downtown Toronto with city views.",
                        PricePerNight = 200,
                        Capacity = 3,
                        Bedrooms = 2,
                        Bathrooms = 1,
                        Type = "Apartment",
                        ImageUrl = "/images/stays/apartment7.jpg",
                        Rating = 4.6m,
                        IsAvailable = true,
                        CreatedAt = DateTime.Now
                    },
                    new Accommodation
                    {
                        Title = "Iceland Northern Lights Cabin",
                        Location = "Reykjavik, Iceland",
                        Address = "Aurora Road 4",
                        Description = "Cabin perfect for viewing the Northern Lights.",
                        PricePerNight = 250,
                        Capacity = 4,
                        Bedrooms = 2,
                        Bathrooms = 1,
                        Type = "Cabin",
                        ImageUrl = "/images/stays/cabin4.jpg",
                        Rating = 4.9m,
                        IsAvailable = true,
                        CreatedAt = DateTime.Now
                    }
                };

                context.Accommodations.AddRange(accommodations);
                context.SaveChanges();
            }

            // assign owner for seeded accommodations to admin if exists
            var adminUser = context.Users.FirstOrDefault(u => u.Role == "Admin");
            if (adminUser != null)
            {
                foreach (var acc in context.Accommodations)
                {
                    if (acc.OwnerId == 0)
                        acc.OwnerId = adminUser.Id;
                }
                context.SaveChanges();
            }

            
            if (!context.AccommodationImages.Any())
            {
                var cliffsideVilla = context.Accommodations.First(a => a.Title == "Santorini Cliffside Villa");
                var sunsetVilla = context.Accommodations.First(a => a.Title == "Santorini Sunset Villa");
                var alpineCabin = context.Accommodations.First(a => a.Title == "Alpine Mountain Cabin");
                var phuketVilla = context.Accommodations.First(a => a.Title == "Phuket Beach Villa");

                context.AccommodationImages.AddRange(
                    new AccommodationImage { AccommodationId = cliffsideVilla.Id, ImageUrl = "/images/stays/villa1.jpg", IsMain = true, DisplayOrder = 1 },
                    new AccommodationImage { AccommodationId = cliffsideVilla.Id, ImageUrl = "/images/stays/villa1_2.jpg", IsMain = false, DisplayOrder = 2 },
                    new AccommodationImage { AccommodationId = cliffsideVilla.Id, ImageUrl = "/images/stays/villa1_3.jpg", IsMain = false, DisplayOrder = 3 },

                    new AccommodationImage { AccommodationId = sunsetVilla.Id, ImageUrl = "/images/stays/villa2.jpg", IsMain = true, DisplayOrder = 1 },
                    new AccommodationImage { AccommodationId = sunsetVilla.Id, ImageUrl = "/images/stays/villa2_2.jpg", IsMain = false, DisplayOrder = 2 },
                    new AccommodationImage { AccommodationId = sunsetVilla.Id, ImageUrl = "/images/stays/villa2_3.jpg", IsMain = false, DisplayOrder = 3 },

                    new AccommodationImage { AccommodationId = alpineCabin.Id, ImageUrl = "/images/stays/cabin2.jpg", IsMain = true, DisplayOrder = 1 },
                    new AccommodationImage { AccommodationId = alpineCabin.Id, ImageUrl = "/images/stays/cabin2_2.jpg", IsMain = false, DisplayOrder = 2 },
                    new AccommodationImage { AccommodationId = alpineCabin.Id, ImageUrl = "/images/stays/cabin2_3.jpg", IsMain = false, DisplayOrder = 3 },

                    new AccommodationImage { AccommodationId = phuketVilla.Id, ImageUrl = "/images/stays/villa3.jpg", IsMain = true, DisplayOrder = 1 },
                    new AccommodationImage { AccommodationId = phuketVilla.Id, ImageUrl = "/images/stays/villa3_2.jpg", IsMain = false, DisplayOrder = 2 },
                    new AccommodationImage { AccommodationId = phuketVilla.Id, ImageUrl = "/images/stays/villa3_3.jpg", IsMain = false, DisplayOrder = 3 }
                );

                context.SaveChanges();
            }

           
            if (!context.AccommodationAmenities.Any())
            {
                var wifi = context.Amenities.First(a => a.Name == "WiFi");
                var pool = context.Amenities.First(a => a.Name == "Pool");
                var parking = context.Amenities.First(a => a.Name == "Parking");
                var airConditioning = context.Amenities.First(a => a.Name == "Air Conditioning");
                var breakfast = context.Amenities.First(a => a.Name == "Breakfast included");
                var kitchen = context.Amenities.First(a => a.Name == "Kitchen");
                var tv = context.Amenities.First(a => a.Name == "TV");
                var petFriendly = context.Amenities.First(a => a.Name == "Pet friendly");

                var cliffsideVilla = context.Accommodations.First(a => a.Title == "Santorini Cliffside Villa");
                var sunsetVilla = context.Accommodations.First(a => a.Title == "Santorini Sunset Villa");
                var lakeComo = context.Accommodations.First(a => a.Title == "Lake Como Luxury Apartment");
                var parisStudio = context.Accommodations.First(a => a.Title == "Paris Boutique Studio");
                var alpineCabin = context.Accommodations.First(a => a.Title == "Alpine Mountain Cabin");
                var barcelona = context.Accommodations.First(a => a.Title == "Barcelona Beach Apartment");
                var berlin = context.Accommodations.First(a => a.Title == "Berlin City Loft");
                var carpathian = context.Accommodations.First(a => a.Title == "Carpathian Forest Cabin");
                var london = context.Accommodations.First(a => a.Title == "London Modern Flat");
                var dubai = context.Accommodations.First(a => a.Title == "Dubai Marina Luxury Suite");
                var phuket = context.Accommodations.First(a => a.Title == "Phuket Beach Villa");
                var tokyo = context.Accommodations.First(a => a.Title == "Tokyo Minimal Apartment");
                var newYork = context.Accommodations.First(a => a.Title == "New York Skyline Studio");
                var toronto = context.Accommodations.First(a => a.Title == "Toronto Downtown Condo");
                var iceland = context.Accommodations.First(a => a.Title == "Iceland Northern Lights Cabin");

                context.AccommodationAmenities.AddRange(
                    new AccommodationAmenity { AccommodationId = cliffsideVilla.Id, AmenityId = wifi.Id },
                    new AccommodationAmenity { AccommodationId = cliffsideVilla.Id, AmenityId = pool.Id },
                    new AccommodationAmenity { AccommodationId = cliffsideVilla.Id, AmenityId = airConditioning.Id },

                    new AccommodationAmenity { AccommodationId = sunsetVilla.Id, AmenityId = wifi.Id },
                    new AccommodationAmenity { AccommodationId = sunsetVilla.Id, AmenityId = pool.Id },
                    new AccommodationAmenity { AccommodationId = sunsetVilla.Id, AmenityId = breakfast.Id },

                    new AccommodationAmenity { AccommodationId = lakeComo.Id, AmenityId = wifi.Id },
                    new AccommodationAmenity { AccommodationId = lakeComo.Id, AmenityId = kitchen.Id },
                    new AccommodationAmenity { AccommodationId = lakeComo.Id, AmenityId = tv.Id },

                    new AccommodationAmenity { AccommodationId = parisStudio.Id, AmenityId = wifi.Id },
                    new AccommodationAmenity { AccommodationId = parisStudio.Id, AmenityId = tv.Id },

                    new AccommodationAmenity { AccommodationId = alpineCabin.Id, AmenityId = wifi.Id },
                    new AccommodationAmenity { AccommodationId = alpineCabin.Id, AmenityId = parking.Id },

                    new AccommodationAmenity { AccommodationId = barcelona.Id, AmenityId = wifi.Id },
                    new AccommodationAmenity { AccommodationId = barcelona.Id, AmenityId = airConditioning.Id },

                    new AccommodationAmenity { AccommodationId = berlin.Id, AmenityId = wifi.Id },
                    new AccommodationAmenity { AccommodationId = berlin.Id, AmenityId = kitchen.Id },

                    new AccommodationAmenity { AccommodationId = carpathian.Id, AmenityId = parking.Id },
                    new AccommodationAmenity { AccommodationId = carpathian.Id, AmenityId = petFriendly.Id },

                    new AccommodationAmenity { AccommodationId = london.Id, AmenityId = wifi.Id },
                    new AccommodationAmenity { AccommodationId = london.Id, AmenityId = tv.Id },

                    new AccommodationAmenity { AccommodationId = dubai.Id, AmenityId = wifi.Id },
                    new AccommodationAmenity { AccommodationId = dubai.Id, AmenityId = pool.Id },
                    new AccommodationAmenity { AccommodationId = dubai.Id, AmenityId = airConditioning.Id },

                    new AccommodationAmenity { AccommodationId = phuket.Id, AmenityId = wifi.Id },
                    new AccommodationAmenity { AccommodationId = phuket.Id, AmenityId = pool.Id },
                    new AccommodationAmenity { AccommodationId = phuket.Id, AmenityId = breakfast.Id },

                    new AccommodationAmenity { AccommodationId = tokyo.Id, AmenityId = wifi.Id },
                    new AccommodationAmenity { AccommodationId = tokyo.Id, AmenityId = airConditioning.Id },

                    new AccommodationAmenity { AccommodationId = newYork.Id, AmenityId = wifi.Id },
                    new AccommodationAmenity { AccommodationId = newYork.Id, AmenityId = tv.Id },

                    new AccommodationAmenity { AccommodationId = toronto.Id, AmenityId = wifi.Id },
                    new AccommodationAmenity { AccommodationId = toronto.Id, AmenityId = kitchen.Id },

                    new AccommodationAmenity { AccommodationId = iceland.Id, AmenityId = wifi.Id },
                    new AccommodationAmenity { AccommodationId = iceland.Id, AmenityId = parking.Id }
                );

                context.SaveChanges();
            }
        }
    }
}