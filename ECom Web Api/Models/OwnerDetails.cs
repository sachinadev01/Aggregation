using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ECom_Web_Api.Models
{
    public class FamilyDetail
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        public ICollection<FamilyChild> Children { get; set; } = new List<FamilyChild>();
    }

    public class FamilyChild
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(10)]
        public string Gender { get; set; } = string.Empty;

        [Required]
        public DateTime Dob { get; set; }

        public string? Photo { get; set; }

        public int FamilyDetailId { get; set; }

        [JsonIgnore]   // 👈 Swagger se hatao
        public FamilyDetail? FamilyDetail { get; set; }   // 👈 nullable bana diya

        public ICollection<FamilyIdentity> Identities { get; set; } = new List<FamilyIdentity>();
    }
    public class FamilyIdentity
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string IdentityType { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string IdentityNumber { get; set; } = string.Empty;

        public int FamilyChildId { get; set; }

        [JsonIgnore]   // 👈 Swagger se hatao
        public FamilyChild? FamilyChild { get; set; }   // 👈 nullable bana diya
    }

    public class FamilyDetailDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public List<FamilyChildDto> Children { get; set; } = new();
    }

    public class FamilyChildDto
    {
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime Dob { get; set; }
        public string? Photo { get; set; }
        public List<FamilyIdentityDto> Identities { get; set; } = new();
    }

    public class FamilyIdentityDto
    {
        public string IdentityType { get; set; } = string.Empty;
        public string IdentityNumber { get; set; } = string.Empty;
    }


}
