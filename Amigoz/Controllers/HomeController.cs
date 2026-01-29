using Amigoz.Data;
using Amigoz.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Amigoz.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context,
            UserManager<AppUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        // =========================
        // Home Page - show last message
        // =========================
        public async Task<IActionResult> Index()
        {
            var lastMessage = await _context.Messages
                .OrderByDescending(m => m.SentAt)
                .FirstOrDefaultAsync();

            ViewBag.LastMessage = lastMessage;

            return View();
        }

        // =========================
        // Chat page - load recent messages and pass to view
        // =========================
        [Authorize]
        public async Task<IActionResult> Chat()
        {
            // Load the last 200 messages (most recent), then order them ascending for display
            var messages = await _context.Messages
                .OrderByDescending(m => m.SentAt)
                .Take(200)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            return View(messages);
        }

        // =========================
        // POST: Save new message (AJAX-friendly)
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create(string text)
        {
            // Ensure user is logged in
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(); // Only logged-in users
            }

            // Create new message
            var message = new Message
            {
                UserId = user.Id,
                Username = user.UserName ?? "Unknown",
                Text = text,
                SentAt = DateTime.UtcNow
            };

            // Save to database
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Return JSON for AJAX requests
            return Json(new
            {
                success = true,
                username = message.Username,
                text = message.Text,
                sentAt = message.SentAt
            });
        }

        // =========================
        // Privacy page
        // =========================
        public IActionResult Privacy()
        {
            return View();
        }

        // =========================
        // Error page
        // =========================
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}