namespace ECom_Web_Api.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ICollection<User> Users { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; }

    }
}
