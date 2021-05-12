using Atheer.Models;

namespace Atheer.Services.UsersService.Models.LoginAttempts
{
    public record ProceedLoginAttemptResponse(User User) : LoginAttemptResponse(User);
}