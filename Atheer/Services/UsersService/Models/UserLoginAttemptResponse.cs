using System;
using Atheer.Models;

namespace Atheer.Services.UsersService.Models
{
    public record UserLoginAttemptResponse(User User, UserLoginAttemptStatus AttemptStatus, int LoginAttemptsLeft, DateTime? NextLoginAttemptTime);
}