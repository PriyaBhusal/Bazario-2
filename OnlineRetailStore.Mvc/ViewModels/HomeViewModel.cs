using System.Collections.Generic;

namespace OnlineRetailStore.Mvc.ViewModels
{
    public class CategoryCardViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
    }

    public class TestimonialViewModel
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
        public string Username { get; set; }
    }

    public class ProductCardViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string ShortDescription { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string ImageUrl { get; set; }
        public double AvgRating { get; set; }
        public int RatingCount { get; set; }
    }

    public class NotificationViewModel
    {
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public System.DateTime CreatedAt { get; set; }
    }

    public class HomeViewModel
    {
        public bool IsLoggedIn { get; set; }
        public int StatCustomers { get; set; }
        public int StatStock { get; set; }
        public int StatDelivered { get; set; }
        public List<CategoryCardViewModel> Categories { get; set; } = new List<CategoryCardViewModel>();
        public List<TestimonialViewModel> Testimonials { get; set; } = new List<TestimonialViewModel>();
        public List<ProductCardViewModel> FeaturedProducts { get; set; } = new List<ProductCardViewModel>();
        public List<NotificationViewModel> Notifications { get; set; } = new List<NotificationViewModel>();
        public int UnreadCount { get; set; }
    }
}
