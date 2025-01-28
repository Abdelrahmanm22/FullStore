
using System.ComponentModel.DataAnnotations;

namespace Store.Models
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "The Delivary Address is required.")]
        [MaxLength(100)]
        public string DeliveryAddress { get; set; } = "";
        public string PaymentMethod { get; set; } = "";
    }
}
