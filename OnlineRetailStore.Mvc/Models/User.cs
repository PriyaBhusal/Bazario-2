using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace OnlineRetailStore.Mvc.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("username")]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [Column("passwordhash")]
        [StringLength(128)]
        public string PasswordHash { get; set; }

        [Required]
        [Column("email")]
        [StringLength(255)]
        public string Email { get; set; }

        [Column("role")]
        [StringLength(50)]
        public string Role { get; set; } = "User";

        [Column("is_approved")]
        public bool IsApproved { get; set; } = true;

        [Column("phone")]
        [StringLength(30)]
        public string Phone { get; set; }

        [Column("address")]
        [StringLength(500)]
        public string Address { get; set; }

        [Column("created_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Product> VendorProducts { get; set; }
    }
}
