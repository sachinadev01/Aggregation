using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ECom_Web_Api.Models
{
    public class Reviews
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [Required]
        [Range(1, 5)]
        public decimal Rating { get; set; }

        public string ReviewText { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
