namespace LivingSpaces.Models
{
    public class Tour
    {
        public int TourID { get; set; }
        public int PropertyListingID { get; set; } 
        public string UserID { get; set; } 
        public DateTime ScheduledDate { get; set; }
        public string Status { get; set; } 
        public string Comments { get; set; }

        public PropertyListing PropertyListing { get; set; }
        public ApplicationUser User { get; set; }
    }

}
