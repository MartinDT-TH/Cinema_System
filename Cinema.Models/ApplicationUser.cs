
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Cinema.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; }
        [NotMapped]
        public string Role { get; set; }

        public string? UserImage { get; set; }
        public int Points { get; set; } = 0;
    }
}