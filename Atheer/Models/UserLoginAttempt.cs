using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Atheer.Models
{
    public class UserLoginAttempt
    {
        public string UserId { get; set; }
        public DateTime AttemptAt { get; set; }
        public string ReferenceId { get; set; }
        public bool SuccessfulLogin { get; set; }
    }
}