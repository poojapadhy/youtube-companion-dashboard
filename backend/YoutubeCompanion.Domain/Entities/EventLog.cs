using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class EventLog
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string EventType { get; set; }   // e.g., "VIDEO_UPDATE"
    public Object Message { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}