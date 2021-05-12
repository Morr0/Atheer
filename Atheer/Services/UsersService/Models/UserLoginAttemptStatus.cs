namespace Atheer.Services.UsersService.Models
{
    public enum UserLoginAttemptStatus : byte
    {
        LoggedIn,
        InvalidCredentials,
        ExceededAttempts
    }
}