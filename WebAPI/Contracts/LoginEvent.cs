using MongoDB.Bson.Serialization.Attributes;

namespace Contracts;
[BsonIgnoreExtraElements]
public record LoginEvent
{
    public string UserId { get; set; }
    public DateTime Timestamp { get; set; }
}

