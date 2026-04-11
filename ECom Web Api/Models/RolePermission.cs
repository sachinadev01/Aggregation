namespace ECom_Web_Api.Models
{
    public class RolePermission
    {
        public int Id { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }

        public int MenuId { get; set; }
        public Menu Menu { get; set; }

        public bool IsVisible { get; set; }
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }

    }
}
