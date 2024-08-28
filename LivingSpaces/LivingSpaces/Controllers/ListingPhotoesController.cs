using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LivingSpaces.Models;

namespace LivingSpaces.Controllers
{
    public class ListingPhotoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ListingPhotoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ListingPhotoes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ListingPhoto.Include(l => l.PropertyListing);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ListingPhotoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listingPhoto = await _context.ListingPhoto
                .Include(l => l.PropertyListing)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (listingPhoto == null)
            {
                return NotFound();
            }

            return View(listingPhoto);
        }

        public IActionResult UploadImages(int propertyListingID)
        {
            ViewBag.PropertyListingID = propertyListingID;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImages(int propertyListingID, List<IFormFile> PhotoFiles)
        {
            if (PhotoFiles != null && PhotoFiles.Count > 0)
            {
                var existingPhotos = _context.ListingPhoto
                    .Where(lp => lp.PropertyListingID == propertyListingID)
                    .ToList();

                foreach (var photo in existingPhotos)
                {
                    _context.ListingPhoto.Remove(photo);

                    var filePath = Path.Combine("wwwroot/uploads", Path.GetFileName(photo.PhotoUrl));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                await _context.SaveChangesAsync();

                foreach (var file in PhotoFiles)
                {
                    if (file.Length > 0)
                    {
                        // Generate a unique file name
                        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                        // Define the file path
                        var filePath = Path.Combine("wwwroot/uploads", uniqueFileName);

                        // Save the file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Add the new photo record to the database
                        var listingPhoto = new ListingPhoto
                        {
                            PhotoUrl = "/uploads/" + uniqueFileName,
                            PropertyListingID = propertyListingID
                        };
                        _context.Add(listingPhoto);
                    }
                }

                // Save changes to add new photos to the database
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("CreateBooking", "Bookings", new { propertyListingID = propertyListingID });
        }




        // GET: ListingPhotoes/Create
        public IActionResult Create()
        {
            ViewData["PropertyListingID"] = new SelectList(_context.Properties, "PropertyListingID", "Address");
            return View();
        }

        // POST: ListingPhotoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int PropertyListingID, List<IFormFile> PhotoFiles)
        {
            if (ModelState.IsValid)
            {
                foreach (var file in PhotoFiles)
                {
                    if (file.Length > 0)
                    {
                        // Generate a unique file name
                        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                        // Define the file path
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", uniqueFileName);

                        // Save the file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Create a new ListingPhoto object
                        var listingPhoto = new ListingPhoto
                        {
                            PhotoUrl = "/uploads/" + uniqueFileName,
                            PropertyListingID = PropertyListingID
                        };

                        _context.Add(listingPhoto);
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["PropertyListingID"] = new SelectList(_context.Properties, "PropertyListingID", "Address", PropertyListingID);
            return View();
        }




        // GET: ListingPhotoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listingPhoto = await _context.ListingPhoto.FindAsync(id);
            if (listingPhoto == null)
            {
                return NotFound();
            }
            ViewData["PropertyListingID"] = new SelectList(_context.Properties, "PropertyListingID", "Address", listingPhoto.PropertyListingID);
            return View(listingPhoto);
        }

        // POST: ListingPhotoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PhotoUrl,PropertyListingID")] ListingPhoto listingPhoto)
        {
            if (id != listingPhoto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(listingPhoto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ListingPhotoExists(listingPhoto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PropertyListingID"] = new SelectList(_context.Properties, "PropertyListingID", "Address", listingPhoto.PropertyListingID);
            return View(listingPhoto);
        }

        // GET: ListingPhotoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listingPhoto = await _context.ListingPhoto
                .Include(l => l.PropertyListing)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (listingPhoto == null)
            {
                return NotFound();
            }

            return View(listingPhoto);
        }

        // POST: ListingPhotoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var listingPhoto = await _context.ListingPhoto.FindAsync(id);
            if (listingPhoto != null)
            {
                _context.ListingPhoto.Remove(listingPhoto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ListingPhotoExists(int id)
        {
            return _context.ListingPhoto.Any(e => e.Id == id);
        }
    }
}
