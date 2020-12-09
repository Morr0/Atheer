using System;
using AtheerBackend.Models;
using Microsoft.Extensions.Configuration;

namespace AtheerBackend.Utilities
{
    public class AtheerConfig
    {
        // Config keys in appsettings.json or any other doc you use in DynamoDBTables
        private static readonly string DynamoDbTables = "DynamoDBTables";
        private static readonly string PostsTableKey = "Posts";
        private static readonly string ContactsTableKey = "Contacts";

        public static readonly string TtlAttribute = "TTL";
        
        public AtheerConfig(IConfiguration configuration)
        {
            PostsTable = configuration[$"{DynamoDbTables}:{PostsTableKey}"];
            ContactsTable = configuration[$"{DynamoDbTables}:{ContactsTableKey}"];
        }
        
        public string PostsTable { get; }
        public string ContactsTable { get; }
    }
}