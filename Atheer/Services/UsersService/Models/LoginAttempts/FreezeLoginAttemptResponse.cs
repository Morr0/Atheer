using System;
using Atheer.Models;

namespace Atheer.Services.UsersService.Models.LoginAttempts
{
    public record FreezeLoginAttemptResponseResponse(User User, DateTime Until) : LoginAttemptResponse(User);
}