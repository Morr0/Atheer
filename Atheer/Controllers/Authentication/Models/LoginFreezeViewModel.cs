using System;

namespace Atheer.Controllers.Authentication.Models
{
    public record LoginFreezeViewModel(string EmailOrUsername, DateTime Until);
}