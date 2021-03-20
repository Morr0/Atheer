namespace Atheer.Controllers.User.Models
{
    public class UserSettingsViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public string DateCreated { get; set; }
        public bool OAuth { get; set; }
    }
}