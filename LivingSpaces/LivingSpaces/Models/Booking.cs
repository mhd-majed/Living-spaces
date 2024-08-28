using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LivingSpaces.Models
{
    public class Booking
    {
        public int BookingID { get; set; }
        public BookingType BookingType { get; set; }

        public int? DailyBookingID { get; set; }
        public DailyBooking DailyBooking { get; set; }

        public int? WeeklyBookingID { get; set; }
        public WeeklyBooking WeeklyBooking { get; set; }

        [Required]
        public int PropertyListingID { get; set; }

        [ForeignKey("PropertyListingID")]
        public virtual PropertyListing PropertyListing { get; set; }
    }

    public enum BookingType
    {
        NoBooking,
        Daily,
        Weekly
    }


}
