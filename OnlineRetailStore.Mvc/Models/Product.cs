using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineRetailStore.Mvc.Models
{
    [Table("products")]
    public class Product
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        [StringLength(200)]
        public string Name { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

        [Column("vendor_id")]
        public int? VendorId { get; set; }

        [Column("short_description")]
        [StringLength(300)]
        public string ShortDescription { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("price", TypeName = "decimal")]
        public decimal Price { get; set; }

        [Column("stock")]
        public int Stock { get; set; }

        [Column("image_url")]
        [StringLength(500)]
        public string ImageUrl { get; set; }

        [Column("created_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [ForeignKey("VendorId")]
        public virtual User Vendor { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; }
    }
}
