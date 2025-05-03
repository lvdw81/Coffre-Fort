using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace coffre_fort_api.Models
{
    public class PasswordShare
    {
        public int Id { get; set; }

        [Required]
        public int SourceUserId { get; set; } // Celui qui partage ses donnees

        [Required]
        public int SharedWithUserId { get; set; } // Celui qui recevra l'acces

        public DateTime DatePartage { get; set; }

        [ForeignKey(nameof(SourceUserId))]
        public User? SourceUser { get; set; }

        [ForeignKey(nameof(SharedWithUserId))]
        public User? SharedWithUser { get; set; }
    }
}
