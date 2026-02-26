using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using YoutubeCompanion.Application.DTOs;
using YoutubeCompanion.Domain.Entities;
using YoutubeCompanion.Infrastructure.Mongo;

namespace YoutubeCompanion.API.Controllers;

[ApiController]
[Route("api/notes")]
public class NotesController : ControllerBase
{
    private readonly MongoContext _context;
    private readonly IConfiguration _config;

    public NotesController(MongoContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    // Add note
    [HttpPost]
    public async Task<IActionResult> AddNote(CreateNoteDto dto)
    {
        var note = new Note
        {
            VideoId = _config["YouTube:VideoId"],
            Content = dto.Content,
            Tags = dto.Tags
        };

        await _context.Notes.InsertOneAsync(note);
        return Ok(note);
    }

    // Search notes
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? text,
        [FromQuery] string? tag)
    {
        var filter = Builders<Note>.Filter.Empty;

        if (!string.IsNullOrEmpty(text))
        {
            filter &= Builders<Note>.Filter.Regex(
                n => n.Content, new MongoDB.Bson.BsonRegularExpression(text, "i"));
        }

        if (!string.IsNullOrEmpty(tag))
        {
            filter &= Builders<Note>.Filter.AnyEq(
                n => n.Tags, tag);
        }

        var results = await _context.Notes
            .Find(filter)
            .ToListAsync();

        return Ok(results);
    }
}