using System.Data;

namespace ECom_Web_Api.Models
{
    public class AccessUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        // Relation
        public int RoleId { get; set; }
        public Role Role { get; set; }

    }
}
