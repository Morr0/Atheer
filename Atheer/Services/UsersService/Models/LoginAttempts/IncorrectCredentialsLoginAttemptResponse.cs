using Atheer.Models;

namespace Atheer.Services.UsersService.Models.LoginAttempts
{
    public record IncorrectCredentialsLoginAttemptResponse(User User, int AttemptsLeft) : LoginAttemptResponse(User);
}