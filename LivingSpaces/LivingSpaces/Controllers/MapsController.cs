using LivingSpaces.Models;
using LivingSpaces.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace LivingSpaces.Controllers
{
    public class MapsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;


        public MapsController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        public IActionResult Index()
        {
            var apiKey = _configuration["GoogleMaps:ApiKey"];
            ViewData["GoogleMapsApiKey"] = apiKey;
            return View();
        }



        [HttpGet]
        public IActionResult SetLocation(int propertyListingID)
        {
            var apiKey = _configuration["GoogleMaps:ApiKey"];
            ViewData["GoogleMapsApiKey"] = apiKey;
            ViewBag.PropertyListingID = propertyListingID; 
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SetLocation(AddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var propertyListing = await _context.Properties
                    .Include(pl => pl.Address) 
                    .FirstOrDefaultAsync(pl => pl.PropertyListingID == model.PropertyListingID);

                if (propertyListing == null)
                {
                    ModelState.AddModelError("", "Property listing not found.");
                    return View(model);
                }

                Address address;
                if (propertyListing.Address != null)
                {
                    address = propertyListing.Address;
                }
                else
                {
                    address = new Address();
                    _context.Addresses.Add(address);
                }

                address.Street = model.Street;
                address.City = model.City;
                address.State = model.State;
                address.Country = model.Country;
                address.Latitude = model.Latitude;
                address.Longitude = model.Longitude;

                propertyListing.Address = address;
                propertyListing.AddressID = address.Id;

                await _context.SaveChangesAsync();

                TempData["RedirectUrl"] = Url.Action("PropertyDetails", "PropertyListings", new { id = model.PropertyListingID });

                TempData["SuccessMessage"] = "Property listing updated successfully.";
                return RedirectToAction("SetLocation", new { PropertyListingID = model.PropertyListingID });
            }
            return View(model);
        }

    }
}
