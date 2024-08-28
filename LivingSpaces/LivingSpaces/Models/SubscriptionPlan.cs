using System.ComponentModel.DataAnnotations;

namespace LivingSpaces.Models
{
    public class SubscriptionPlan
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string SubscriptionName { get; set; }
        public decimal Price { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [StringLength(1000)]
        public string Description { get; set; }

        public int NumberOfListings { get; set; }

        public string SupportVr { get; set; }
        public int ListingActiveDays { get; set; }
        // Navigation property
        public virtual ICollection<UserSubscription>? UserSubscriptions { get; set; }
    }

}
