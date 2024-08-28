using System;
using System.ComponentModel.DataAnnotations;
using LivingSpaces.Models;
namespace LivingSpaces.ViewModels
{
    public class BookingViewModel
    {
        public int BookingID { get; set; }
        public BookingType BookingType { get; set; }

        // Daily Booking fields
        public TimeSpan? DailyStartTime { get; set; }
        public TimeSpan? DailyEndTime { get; set; }

        // Weekly Booking fields
        public TimeSpan? MondayStartTime { get; set; }
        public TimeSpan? MondayEndTime { get; set; }
        public TimeSpan? TuesdayStartTime { get; set; }
        public TimeSpan? TuesdayEndTime { get; set; }
        public TimeSpan? WednesdayStartTime { get; set; }
        public TimeSpan? WednesdayEndTime { get; set; }
        public TimeSpan? ThursdayStartTime { get; set; }
        public TimeSpan? ThursdayEndTime { get; set; }
        public TimeSpan? FridayStartTime { get; set; }
        public TimeSpan? FridayEndTime { get; set; }
        public TimeSpan? SaturdayStartTime { get; set; }
        public TimeSpan? SaturdayEndTime { get; set; }
        public TimeSpan? SundayStartTime { get; set; }
        public TimeSpan? SundayEndTime { get; set; }

        [Required]
        public int PropertyListingID { get; set; }
    }
}
