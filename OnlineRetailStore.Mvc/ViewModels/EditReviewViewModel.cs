using System.ComponentModel.DataAnnotations;

namespace OnlineRetailStore.Mvc.ViewModels
{
    public class EditReviewViewModel
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public bool Eligible { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; } = 5;

        [StringLength(1000)]
        public string Comment { get; set; }
    }
}
