using Microsoft.AspNetCore.SignalR;
using LivingSpaces.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System;

public class ChatHub : Hub
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ChatHub(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task SendMessage(string recipientUserId, string message)
    {
        var senderUserId = Context.UserIdentifier;

        var sender = await _userManager.FindByIdAsync(senderUserId);
        var recipient = await _userManager.FindByIdAsync(recipientUserId);

        if (sender == null || recipient == null)
        {
            return;
        }

        var newMessage = new Text
        {
            RecipientId = recipientUserId,
            SenderId = senderUserId,
            Content = message,
            SentAt = DateTime.UtcNow
        };
        _context.Texts.Add(newMessage);
        await _context.SaveChangesAsync();


        Console.WriteLine(senderUserId);

        await Clients.User(recipientUserId).SendAsync("ReceiveMessage", senderUserId, message);
        await Clients.User(senderUserId).SendAsync("ReceiveMessage", senderUserId, message);
    }
}
