using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace coffre_fort_api.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Identifiant { get; set; }

        [Required]
        public string MotDePasseHash { get; set; }

        public string Role { get; set; } = "user";

        public List<PasswordEntry> PasswordEntries { get; set; } = new();
    }
}
