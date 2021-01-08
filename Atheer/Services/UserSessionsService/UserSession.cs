namespace Atheer.Services.UserSessionsService
{
    public class UserSession
    {
        public UserSession(string id, string userId)
        {
            Id = id;
            UserId = userId;
        }
        
        public string Id { get; }
        public string UserId { get; }
    }
}