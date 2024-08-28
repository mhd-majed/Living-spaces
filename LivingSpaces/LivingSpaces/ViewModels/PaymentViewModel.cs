using System;
using System.ComponentModel.DataAnnotations;

namespace LivingSpaces.Models
{
    public class PaymentViewModel
    {
        [Required(ErrorMessage = "Card Number is required.")]
        [StringLength(16, MinimumLength = 13, ErrorMessage = "Card Number must be between 13 and 16 digits.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Card Number must be numeric.")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Cardholder Name is required.")]
        public string CardholderName { get; set; }

        [Required(ErrorMessage = "Expiry Date is required.")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$", ErrorMessage = "Expiry Date must be in MM/YY format.")]
        public string ExpiryDate { get; set; }

        [Required(ErrorMessage = "CVV is required.")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "3 digits")]

        [RegularExpression(@"^\d+$", ErrorMessage = "Card Number must be numeric.")]
        public string CVVNumber { get; set; }

        [Required(ErrorMessage = "State is required.")]
        public string State { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Street is required.")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Postal code is required.")]
        public string PostalCode { get; set; }

        // New field for Subscription Plan ID
        public int SubscriptionPlanId { get; set; }
    }
}
