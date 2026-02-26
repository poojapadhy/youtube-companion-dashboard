using YoutubeCompanion.Infrastructure.Mongo;

public class EventLogger
{
    private readonly MongoContext _ctx;
    public EventLogger(MongoContext ctx) => _ctx = ctx;

    public Task LogAsync(string type, Object message) =>
        _ctx.EventLogs.InsertOneAsync(new EventLog
        {
            EventType = type,
            Message = message
        });
}