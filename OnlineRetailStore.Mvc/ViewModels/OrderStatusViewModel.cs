using System;
using System.Collections.Generic;

namespace OnlineRetailStore.Mvc.ViewModels
{
    public class OrderStatusItemViewModel
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public int? ExistingRating { get; set; }
        public bool ShowRate { get; set; }
    }

    public class OrderStatusViewModel
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderStatusItemViewModel> Items { get; set; } = new List<OrderStatusItemViewModel>();

        public string StatusBadgeClass
        {
            get
            {
                switch (Status)
                {
                    case "Pending": return "bg-secondary";
                    case "Sent out for delivery": return "bg-info text-dark";
                    case "Delivered": return "bg-success";
                    case "Cancelled": return "bg-danger";
                    default: return "bg-secondary";
                }
            }
        }

        public string StepClass(int step)
        {
            int current = Status == "Pending" ? 1 : Status == "Sent out for delivery" ? 2 : Status == "Delivered" ? 3 : 0;
            if (step < current) return "bg-success text-white";
            if (step == current) return "bg-primary text-white";
            return "bg-light text-muted";
        }
    }
}
