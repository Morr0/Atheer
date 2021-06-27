using MongoDB.Bson.Serialization.Attributes;

namespace Atheer.Models
{
    public class NavItem
    {
        [BsonId]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}