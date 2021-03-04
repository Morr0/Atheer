namespace Atheer.Utilities.Config.Models
{
    public class Recaptcha
    {
        public bool Enabled { get; set; }
        public string SiteKey { get; set; }
        public string SecretKey { get; set; }
    }
}