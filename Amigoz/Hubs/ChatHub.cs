using Amigoz.Data;
using Amigoz.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Amigoz.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        // Inject DbContext and UserManager
        public ChatHub(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Server method to receive message from client
        public async Task SendMessage(string text)
        {
            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(Context.User);
            if (user == null)
            {
                // If no logged-in user, ignore message
                return;
            }

            // Create a new message
            var message = new Message
            {
                UserId = user.Id,
                Username = user.UserName ?? "Unknown",
                Text = text,
                SentAt = DateTime.UtcNow
            };

            // Save message to database
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Broadcast message to all connected clients
            await Clients.All.SendAsync("ReceiveMessage", user.UserName, text, message.SentAt);
        }
    }
}
