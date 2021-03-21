namespace Atheer.Services.OAuthService
{
    public class OAuthUserInfo
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string OAuthProvider { get; set; }
        public string OAuthProviderId { get; set; }
        public string OAuthUsername { get; set; }
    }
}