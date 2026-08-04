using System.Collections.Generic;
using System.Linq;
using OnlineRetailStore.Mvc.Models;
using OnlineRetailStore.Mvc.ViewModels;

namespace OnlineRetailStore.Mvc.Services
{
    /// <summary>Badges are computed on the fly from existing orders/ratings - no extra table needed.</summary>
    public static class BadgeService
    {
        public static List<BadgeViewModel> GetBadges(AppDbContext db, int userId)
        {
            int orderCount = db.Orders.Count(o => o.UserId == userId && o.Status != "Cancelled");
            decimal totalSpent = db.Orders.Where(o => o.UserId == userId && o.Status != "Cancelled").Sum(o => (decimal?)o.Total) ?? 0;
            int reviewCount = db.Ratings.Count(r => r.UserId == userId);

            return new List<BadgeViewModel>
            {
                new BadgeViewModel { Icon = "🎉", Name = "First Purchase", Description = "Placed your first order", Earned = orderCount >= 1 },
                new BadgeViewModel { Icon = "🛍️", Name = "Frequent Buyer", Description = "Placed 5 orders", Earned = orderCount >= 5 },
                new BadgeViewModel { Icon = "👑", Name = "Loyal Customer", Description = "Placed 10 orders", Earned = orderCount >= 10 },
                new BadgeViewModel { Icon = "💰", Name = "Big Spender", Description = "Spent Rs. 10,000+ in total", Earned = totalSpent >= 10000 },
                new BadgeViewModel { Icon = "⭐", Name = "Top Reviewer", Description = "Left 5 product reviews", Earned = reviewCount >= 5 }
            };
        }

        public static int EarnedCount(AppDbContext db, int userId)
        {
            return GetBadges(db, userId).Count(b => b.Earned);
        }
    }
}
