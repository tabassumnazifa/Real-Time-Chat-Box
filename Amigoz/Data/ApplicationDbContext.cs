using Amigoz.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Amigoz.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        // Constructor
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Table for messages
        public DbSet<Message> Messages { get; set; } = null!; // null! to satisfy non-nullable requirement

        // LINQ QUERY: Get all messages ordered by time
        public IQueryable<Message> GetAllMessages()
        {
            return Messages
                   .OrderBy(m => m.SentAt);
        }

        // LINQ QUERY: Get messages by a specific user
        public IQueryable<Message> GetMessagesByUser(string userId)
        {
            return Messages
                   .Where(m => m.UserId == userId)
                   .OrderBy(m => m.SentAt);
        }
    }
}
