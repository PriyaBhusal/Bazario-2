using System.ComponentModel.DataAnnotations;

namespace OnlineRetailStore.Mvc.Areas.Vendor.Models
{
    public class VendorProductVm
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

        public string Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; }
    }

    public class VendorDashboardVm
    {
        public bool IsApproved { get; set; }
        public int StatProducts { get; set; }
        public int StatStock { get; set; }
        public int StatOutOfStock { get; set; }
        public string StatValue { get; set; }
        public System.Collections.Generic.List<VendorProductVm> Products { get; set; } = new System.Collections.Generic.List<VendorProductVm>();
    }
}
