using Microsoft.Extensions.Configuration;

namespace Atheer.Utilities
{
    public class AtheerConfig
    {
        private static readonly string TitleKey = "Title";
        private static readonly string DescriptionKey = "Description";
        // Config keys in appsettings.json or any other doc you use in DynamoDBTables
        private static readonly string DynamoDbTables = "DynamoDBTables";
        private static readonly string PostsTableKey = "Posts";
        private static readonly string ContactsTableKey = "Contacts";

        public static readonly string TtlAttribute = "TTL";
        
        public AtheerConfig(IConfiguration configuration)
        {
            Title = configuration[$"{TitleKey}"];
            Description = configuration[$"{Description}"];
            PostsTable = configuration[$"{DynamoDbTables}:{PostsTableKey}"];
            ContactsTable = configuration[$"{DynamoDbTables}:{ContactsTableKey}"];
        }

        public string Title { get; }
        public string Description { get; }
        
        public string PostsTable { get; }
        public string ContactsTable { get; }
    }
}