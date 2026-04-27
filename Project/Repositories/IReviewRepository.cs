using Project.Models;

namespace Project.Repositories;

public interface IReviewRepository
{
    List<Review> GetByAccommodationId(int accommodationId);
    bool HasUserReviewed(int userId, int accommodationId);
    void Add(Review review);
    void SaveChanges();
    Dictionary<int, decimal> GetAverageRatings(IEnumerable<int> accommodationIds);
}
