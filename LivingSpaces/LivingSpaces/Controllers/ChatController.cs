using Microsoft.AspNetCore.Mvc;
using LivingSpaces.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
public class ChatController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ChatController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Chat/Index
    public async Task<IActionResult> Index(string selectedUserId = null)
    {
        var currentUserId = _userManager.GetUserId(User);

        var interactedUserIds = await _context.Texts
            
            .Where(m => m.SenderId == currentUserId || m.RecipientId == currentUserId)
            .Select(m => m.SenderId == currentUserId ? m.RecipientId : m.SenderId)
            .Distinct()
            .ToListAsync();

        var interactedUsers = await _context.Users
            .Where(u => interactedUserIds.Contains(u.Id))
            .ToListAsync();

        foreach (var user in interactedUsers)
        {
            Console.WriteLine(user.Id);
        }

        List<Text> messages = new List<Text>();
        if (!string.IsNullOrEmpty(selectedUserId))
        {
            messages = await _context.Texts
                .Where(m => (m.SenderId == currentUserId && m.RecipientId == selectedUserId) ||
                            (m.SenderId == selectedUserId && m.RecipientId == currentUserId))
                .OrderBy(m => m.SentAt)
                .Include(m => m.Sender)
                .ToListAsync();
        }

        ViewBag.Messages = messages;
        ViewBag.SelectedUserId = selectedUserId;

        return View(interactedUsers);
    }

}

