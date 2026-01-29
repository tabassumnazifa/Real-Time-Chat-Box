using Amigoz.Models;
using System.ComponentModel.DataAnnotations;

namespace Amigoz.Models
{
    public class Message
    {
        public int Id { get; set; }

        // Setting to string.Empty fixes the "non-nullable property" warning
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Text { get; set; } = string.Empty;

        public DateTime SentAt { get; set; } = DateTime.Now;

        public string? UserId { get; set; }
        public virtual AppUser? User { get; set; }
    }
}