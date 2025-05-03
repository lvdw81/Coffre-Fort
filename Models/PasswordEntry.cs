using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace coffre_fort_api.Models
{
    public class PasswordEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string NomApplication { get; set; }

        [Required]
        public string Identifiant { get; set; }

        [Required]
        public string MotDePasse { get; set; }

        public List<string> Tags { get; set; } = new();

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User? User { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
    }
}
