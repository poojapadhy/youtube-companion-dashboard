using MongoDB.Driver;
using YoutubeCompanion.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace YoutubeCompanion.Infrastructure.Mongo;

public class MongoContext
{
    public IMongoCollection<Note> Notes { get; }

    public IMongoCollection<EventLog> EventLogs { get; }

    public MongoContext(IConfiguration config)
    {
        var client = new MongoClient(
            config["MongoDb:ConnectionString"]);

        var database = client.GetDatabase(
            config["MongoDb:DatabaseName"]);

        Notes = database.GetCollection<Note>(
            config["MongoDb:NotesCollection"]);

        EventLogs = database.GetCollection<EventLog>("event_logs");
    }
}