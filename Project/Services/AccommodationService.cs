using Project.Models;
using Project.Repositories;

namespace Project.Services;

public class AccommodationService : IAccommodationService
{
    private readonly IAccommodationRepository _accommodationRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IUnavailablePeriodRepository _unavailablePeriodRepository;

    public AccommodationService(
        IAccommodationRepository accommodationRepository,
        IBookingRepository bookingRepository,
        IReviewRepository reviewRepository,
        IUnavailablePeriodRepository unavailablePeriodRepository)
    {
        _accommodationRepository = accommodationRepository;
        _bookingRepository = bookingRepository;
        _reviewRepository = reviewRepository;
        _unavailablePeriodRepository = unavailablePeriodRepository;
    }

    public List<Accommodation> GetAllForAdmin() => _accommodationRepository.GetAllForAdmin();

    public List<Accommodation> Search(string? location, string? type, decimal? maxPrice, int? guests, string? checkIn, string? checkOut)
    {
        var query = _accommodationRepository.GetAll().AsQueryable();
        var requestedCheckIn = DateTime.MinValue;
        var requestedCheckOut = DateTime.MinValue;
        var hasDates = DateTime.TryParse(checkIn, out requestedCheckIn) && DateTime.TryParse(checkOut, out requestedCheckOut) && requestedCheckIn < requestedCheckOut;

        if (!string.IsNullOrWhiteSpace(location))
        {
            location = location.Trim().ToLower();
            query = query.Where(a => a.Location.ToLower().Contains(location));
        }

        if (!string.IsNullOrWhiteSpace(type))
        {
            query = query.Where(a => a.Type.ToLower() == type.ToLower());
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(a => a.PricePerNight <= maxPrice.Value);
        }

        if (guests.HasValue)
        {
            query = query.Where(a => a.Capacity >= guests.Value);
        }

        if (hasDates)
        {
            query = query.Where(a => !_bookingRepository.HasOverlappingBooking(a.Id, requestedCheckIn, requestedCheckOut)
                && !_unavailablePeriodRepository.HasOverlap(a.Id, requestedCheckIn, requestedCheckOut));
        }

        var stays = query.ToList();
        var ratingsByAccommodationId = _reviewRepository.GetAverageRatings(stays.Select(a => a.Id));

        foreach (var stay in stays)
        {
            if (ratingsByAccommodationId.TryGetValue(stay.Id, out var averageRating))
            {
                stay.Rating = averageRating;
            }
        }

        return stays.OrderByDescending(a => a.Rating).ThenByDescending(a => a.CreatedAt).ToList();
    }

    public AccommodationDetailsViewModel? GetDetails(int id, int? currentUserId)
    {
        var accommodation = _accommodationRepository.GetById(id);
        if (accommodation == null)
        {
            return null;
        }

        var amenities = _accommodationRepository.GetAmenities(id);
        var images = _accommodationRepository.GetImages(id);
        var reviews = _accommodationRepository.GetReviews(id);

        var ratingsByAccommodationId = _reviewRepository.GetAverageRatings(new[] { id });
        if (ratingsByAccommodationId.TryGetValue(id, out var averageRating))
        {
            accommodation.Rating = averageRating;
        }

        var canLeaveReview = false;
        var hasReviewed = false;

        if (currentUserId.HasValue)
        {
            hasReviewed = _reviewRepository.HasUserReviewed(currentUserId.Value, id);
            canLeaveReview = !hasReviewed && _bookingRepository.HasUserStayed(currentUserId.Value, id);
        }

        return new AccommodationDetailsViewModel
        {
            Accommodation = accommodation,
            Amenities = amenities,
            Images = images,
            Reviews = reviews,
            CanLeaveReview = canLeaveReview,
            HasReviewed = hasReviewed
        };
    }

    public Accommodation? GetById(int id) => _accommodationRepository.GetById(id);

    public void Create(Accommodation accommodation, int ownerId)
    {
        accommodation.CreatedAt = DateTime.Now;
        accommodation.OwnerId = ownerId;
        _accommodationRepository.Add(accommodation);
        _accommodationRepository.SaveChanges();
    }

    public void Update(Accommodation accommodation)
    {
        var current = _accommodationRepository.GetById(accommodation.Id);
        if (current == null)
        {
            throw new InvalidOperationException("Accommodation not found.");
        }

        current.Title = accommodation.Title;
        current.Location = accommodation.Location;
        current.Address = accommodation.Address;
        current.Description = accommodation.Description;
        current.PricePerNight = accommodation.PricePerNight;
        current.Capacity = accommodation.Capacity;
        current.Bedrooms = accommodation.Bedrooms;
        current.Bathrooms = accommodation.Bathrooms;
        current.Type = accommodation.Type;
        current.ImageUrl = accommodation.ImageUrl;

        _accommodationRepository.SaveChanges();
    }

    public void Delete(int id)
    {
        var current = _accommodationRepository.GetById(id);
        if (current == null)
        {
            throw new InvalidOperationException("Accommodation not found.");
        }

        _accommodationRepository.Remove(current);
        _accommodationRepository.SaveChanges();
    }

    public void AddReview(int accommodationId, int userId, int rating, string comment)
    {
        if (rating < 1 || rating > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(rating));
        }

        comment = (comment ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(comment))
        {
            throw new ArgumentException("Comment is required.", nameof(comment));
        }

        if (comment.Length > 1000)
        {
            throw new ArgumentException("Comment is too long.", nameof(comment));
        }

        var accommodation = _accommodationRepository.GetById(accommodationId);
        if (accommodation == null)
        {
            throw new InvalidOperationException("Accommodation not found.");
        }

        if (!_bookingRepository.HasUserStayed(userId, accommodationId))
        {
            throw new InvalidOperationException("You can leave a review after your stay.");
        }

        if (_reviewRepository.HasUserReviewed(userId, accommodationId))
        {
            throw new InvalidOperationException("You have already reviewed this accommodation.");
        }

        var review = new Review
        {
            UserId = userId,
            AccommodationId = accommodationId,
            Rating = rating,
            Comment = comment,
            CreatedAt = DateTime.Now
        };

        _reviewRepository.Add(review);
        _reviewRepository.SaveChanges();
    }
}
