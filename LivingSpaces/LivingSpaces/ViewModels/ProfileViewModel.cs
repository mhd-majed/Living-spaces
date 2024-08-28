using System.ComponentModel.DataAnnotations;



namespace LivingSpaces.ViewModels
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string? ProfileImageUrl { get; set; } = "~/assets/images/user.jpg";
    }
}
