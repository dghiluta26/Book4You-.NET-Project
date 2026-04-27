using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext _context;

    public ReviewRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<Review> GetByAccommodationId(int accommodationId)
    {
        return _context.Reviews
            .Include(r => r.User)
            .Where(r => r.AccommodationId == accommodationId)
            .OrderByDescending(r => r.CreatedAt)
            .ToList();
    }

    public bool HasUserReviewed(int userId, int accommodationId)
    {
        return _context.Reviews.Any(r => r.UserId == userId && r.AccommodationId == accommodationId);
    }

    public void Add(Review review) => _context.Reviews.Add(review);

    public void SaveChanges() => _context.SaveChanges();

    public Dictionary<int, decimal> GetAverageRatings(IEnumerable<int> accommodationIds)
    {
        var idList = accommodationIds.Distinct().ToList();
        if (idList.Count == 0)
        {
            return new Dictionary<int, decimal>();
        }

        return _context.Reviews
            .Where(r => idList.Contains(r.AccommodationId))
            .GroupBy(r => r.AccommodationId)
            .Select(g => new
            {
                AccommodationId = g.Key,
                AverageRating = g.Average(r => (decimal)r.Rating)
            })
            .AsEnumerable()
            .ToDictionary(x => x.AccommodationId, x => decimal.Round(x.AverageRating, 2));
    }
}
