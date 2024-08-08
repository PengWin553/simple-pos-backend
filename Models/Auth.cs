namespace simple_pos_backend.Models
{
    public class Register
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    public class Login
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
