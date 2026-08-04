using System.Collections.Generic;

namespace OnlineRetailStore.Mvc.ViewModels
{
    public class ProductFilterViewModel
    {
        public string Label { get; set; }
        public string Category { get; set; }
        public bool Active { get; set; }
    }

    public class ProductListItemViewModel
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

        public string StarsLabel
        {
            get
            {
                if (RatingCount == 0) return "No ratings yet";
                int rounded = (int)System.Math.Round(AvgRating);
                return new string('★', rounded) + new string('☆', 5 - rounded) + $" ({RatingCount})";
            }
        }
    }

    public class ProductIndexViewModel
    {
        public List<ProductFilterViewModel> Filters { get; set; } = new List<ProductFilterViewModel>();
        public List<ProductListItemViewModel> Products { get; set; } = new List<ProductListItemViewModel>();
        public bool IsLoggedIn { get; set; }
    }

    public class ProductDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int Stock { get; set; }
        public int RatingCount { get; set; }
        public double AvgRating { get; set; }
        public List<ReviewViewModel> Reviews { get; set; } = new List<ReviewViewModel>();
        public bool CanAddToCart { get; set; }
    }
}
