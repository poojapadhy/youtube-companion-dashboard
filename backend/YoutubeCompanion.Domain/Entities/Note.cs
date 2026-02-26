using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace YoutubeCompanion.Domain.Entities;

public class Note
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string VideoId { get; set; }

    public string Content { get; set; }

    public List<string> Tags { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}