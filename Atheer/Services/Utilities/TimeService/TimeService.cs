using System;

namespace Atheer.Services.Utilities.TimeService
{
    public class TimeService : ITimeService
    {
        public DateTime Get()
        {
            return DateTime.UtcNow;
        }
    }
}