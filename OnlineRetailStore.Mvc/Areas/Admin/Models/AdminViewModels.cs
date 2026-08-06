using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineRetailStore.Mvc.Areas.Admin.Models
{
    public class CategoryVm
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
    }

    public class ProductAdminVm
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        [StringLength(300)]
        [Display(Name = "Short description")]
        public string ShortDescription { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; }

        public string OwnerName { get; set; }
    }

    public class AdminOrderVm
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Items { get; set; }
    }

    public class VendorUserVm
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserAdminVm
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AdminDashboardVm
    {
        public int TotalProducts { get; set; }
        public int TotalVendors { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSales { get; set; }
        public int LowStockCount { get; set; }

        public List<string> SalesLabels { get; set; } = new List<string>();
        public List<decimal> SalesData { get; set; } = new List<decimal>();

        public List<string> StatusLabels { get; set; } = new List<string>();
        public List<int> StatusData { get; set; } = new List<int>();

        public List<string> TopProductLabels { get; set; } = new List<string>();
        public List<int> TopProductData { get; set; } = new List<int>();
    }
}
