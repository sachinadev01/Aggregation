using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ECom_Web_Api.Models
{
    public class Notification
    {
            [Key]
            public int Id { get; set; }

            [Required]
            public int ServiceUserId { get; set; }

        [Required]
        public string servicetitle { get; set; }

        [Required]
            public int loginid { get; set; }

        [Required]
        public string name { get; set; }

        [Required]
        public string username { get; set; }

        [Required]
        public string email { get; set; }

        [Required]
        public string phone { get; set; }

        [Required]
        public string address { get; set; }

        [Required]
        public string status { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        public int userId { get; set; }


    }
}
