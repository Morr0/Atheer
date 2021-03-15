namespace Atheer.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }

        public string Email { get; set; }
        
        public string PasswordHash { get; set; }
        
        public string DateCreated { get; set; }
        
        public string DateLastLoggedIn { get; set; }

        // Separated by comma
        public string Roles { get; set; }

        public string ImageUrl { get; set; }
    }
}