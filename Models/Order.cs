using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineRetailStore.Mvc.Models
{
    [Table("orders")]
    public class Order
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("status")]
        [StringLength(30)]
        public string Status { get; set; } = "Pending";

        [Column("total", TypeName = "decimal")]
        public decimal Total { get; set; }

        [Column("payment_method")]
        [StringLength(30)]
        public string PaymentMethod { get; set; } = "eSewa";

        [Column("transaction_id")]
        [StringLength(100)]
        public string TransactionId { get; set; }

        [Column("payment_ref")]
        [StringLength(100)]
        public string PaymentRef { get; set; }

        [Column("shipping_address")]
        [StringLength(500)]
        public string ShippingAddress { get; set; }

        [Column("shipping_phone")]
        [StringLength(30)]
        public string ShippingPhone { get; set; }

        [Column("created_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
