namespace AtheerBackend.Models
{
    public class User
    {
        public string OAuthId { get; set; }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string LastOnlineDatetime { get; set; }
        
        public string LastOAuthToken { get; set; }
        public string LastOAuthAccessToken { get; set; }
    }
}