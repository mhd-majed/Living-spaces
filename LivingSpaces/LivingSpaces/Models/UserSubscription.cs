namespace LivingSpaces.Models
{
    public class UserSubscription
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int SubscriptionPlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } =false;
        public int RemainingListings { get; set; }
        public int ListingActiveDays { get; set; }

        // Navigation properties
        public virtual ApplicationUser User { get; set; }
        public virtual SubscriptionPlan SubscriptionPlan { get; set; }
    }

}
