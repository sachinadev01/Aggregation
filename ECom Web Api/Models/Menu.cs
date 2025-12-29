namespace ECom_Web_Api.Models
{
    public class Menu
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string? Route { get; set; }

        // Parent-Child relation
        public int? ParentId { get; set; }
        public Menu? Parent { get; set; }
        public ICollection<Menu> Children { get; set; }

        public int OrderIndex { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<RolePermission> RolePermissions { get; set; }

    }
}
