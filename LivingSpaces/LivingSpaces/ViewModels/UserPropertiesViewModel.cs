using LivingSpaces.Models;

namespace LivingSpaces.ViewModels
{
    public class UserPropertiesViewModel
    {
        public ApplicationUser ApplicationUser { get; set; }
        public List<PropertyListing> Properties { get; set; }
    }

}
