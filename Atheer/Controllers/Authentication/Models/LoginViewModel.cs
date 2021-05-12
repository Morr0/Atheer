namespace Atheer.Controllers.Authentication.Models
{
    public class LoginViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int AttemptsLeft { get; set; }
    }
}