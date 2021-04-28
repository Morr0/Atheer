namespace Atheer.Utilities.Config.Models
{
    public class Site
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public bool ShowLoginButton { get; set; }
        public bool CanRegister { get; set; }
        public string IconUrl { get; set; }
    }
}