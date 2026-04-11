namespace ECom_Web_Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? LoginId { get; set; }
        public string? Password { get; set; }
        public string? phone { get; set; }
        public string? address { get; set; }
        //public string? role { get; set; }
        public string? profile_pic { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }

    public class Login
    {
        public string? LoginId { get; set; }
        public string? Password { get; set; }
    }
}
