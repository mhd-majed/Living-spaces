namespace LivingSpaces.Models
{
    public class DailyBooking
    {
        public int DailyBookingID { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

}
