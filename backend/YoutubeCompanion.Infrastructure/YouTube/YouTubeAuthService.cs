using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.Extensions.Configuration;
using YoutubeCompanion.Application.DTOs;

namespace YoutubeCompanion.Infrastructure.YouTube;

public class YouTubeAuthService
{
    private readonly IConfiguration _config;
    private readonly GoogleAuthorizationCodeFlow _flow;

    private static UserCredential? _cachedCredential;

    public YouTubeAuthService(IConfiguration config)
    {
        _config = config;

        _flow = new GoogleAuthorizationCodeFlow(
            new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = _config["YouTube:ClientId"],
                    ClientSecret = _config["YouTube:ClientSecret"]
                },
                // Include both scopes to allow managing comments. 'youtube.force-ssl' is required for write operations.
                Scopes = new[] { YouTubeService.Scope.Youtube, YouTubeService.Scope.YoutubeForceSsl },
                IncludeGrantedScopes = true
            });
    }

    // STEP 1: Generate Google login URL (WEB OAUTH)
    public string GetLoginUrl()
    {
        var request = (GoogleAuthorizationCodeRequestUrl?)_flow.CreateAuthorizationCodeRequest(
            _config["YouTube:RedirectUri"]
        ) ?? throw new InvalidOperationException("Failed to create Google authorization request.");

        // ✅ Supported in v1.73
        request.Prompt = "consent";
        request.IncludeGrantedScopes = "true";

        return request.Build().AbsoluteUri;
    }

    // STEP 2: Exchange authorization code for tokens
    public async Task<UserCredential> ExchangeCodeForTokenAsync(string code)
    {
        TokenResponse token = await _flow.ExchangeCodeForTokenAsync(
            userId: "youtube-user",
            code: code,
            redirectUri: _config["YouTube:RedirectUri"],
            CancellationToken.None
        );

        return new UserCredential(_flow, "youtube-user", token);
    }

    // STEP 3: Create YouTube client
    public YouTubeService CreateYouTubeService(UserCredential credential)
    {
        return new YouTubeService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "YouTube Companion Dashboard"
        });
    }

    public async Task<VideoDetailsDto> GetVideoDetailsAsync(
    YouTubeService service,
    string videoId)
    {
        var request = service.Videos.List("snippet,statistics");
        request.Id = videoId;

        var response = await request.ExecuteAsync();

        Console.WriteLine($"Items count: {response.Items?.Count}");

        if (response.Items == null || !response.Items.Any())
        {
            throw new Exception(
                $"Video not found. VideoId used: {videoId}");
        }

        var video = response.Items.FirstOrDefault();

        if (video == null)
            throw new Exception("Video not found");

        return new VideoDetailsDto
        {
            VideoId = videoId,
            Title = video.Snippet.Title,
            Description = video.Snippet.Description,
            ViewCount = (long)(video.Statistics.ViewCount ?? 0),
            LikeCount = (long)(video.Statistics.LikeCount ?? 0),
            CommentCount = (long)(video.Statistics.CommentCount ?? 0)
        };
    }

    public async Task UpdateVideoAsync(
    YouTubeService service,
    string videoId,
    string newTitle,
    string newDescription)
    {
        // 1. Fetch existing video
        var listRequest = service.Videos.List("snippet");
        listRequest.Id = videoId;

        var listResponse = await listRequest.ExecuteAsync();
        var video = listResponse.Items.FirstOrDefault();

        if (video == null)
            throw new Exception("Video not found");

        // 2. Modify snippet
        video.Snippet.Title = newTitle;
        video.Snippet.Description = newDescription;

        // 3. Update video
        var updateRequest = service.Videos.Update(video, "snippet");
        await updateRequest.ExecuteAsync();
    }

    public async Task<string> AddCommentAsync(
        YouTubeService service,
        string videoId,
        string text)
    {
        var commentThread = new CommentThread
        {
            Snippet = new CommentThreadSnippet
            {
                VideoId = videoId,
                TopLevelComment = new Comment
                {
                    Snippet = new CommentSnippet
                    {
                        TextOriginal = text
                    }
                }
            }
        };

        var request = service.CommentThreads.Insert(
            commentThread, "snippet");

        var response = await request.ExecuteAsync();
        return response.Id;
    }

    public async Task<string> ReplyToCommentAsync(
    YouTubeService service,
    string parentCommentId,
    string text)
    {
        var comment = new Comment
        {
            Snippet = new CommentSnippet
            {
                ParentId = parentCommentId,
                TextOriginal = text
            }
        };

        var request = service.Comments.Insert(
            comment, "snippet");

        var response = await request.ExecuteAsync();
        return response.Id;
    }

    public async Task DeleteCommentAsync(
    YouTubeService service,
    string commentId)
    {
        var request = service.Comments.Delete(commentId);
        await request.ExecuteAsync();
    }

    public void CacheCredential(UserCredential credential)
    {
        _cachedCredential = credential;
    }

    public UserCredential GetCachedCredential()
    {
        if (_cachedCredential == null)
            throw new Exception("User not authenticated");

        return _cachedCredential;
    }

    public void EnsureCachedCredentialHasScopes(params string[] requiredScopes)
    {
        if (_cachedCredential == null)
            throw new Exception("User not authenticated");

        var granted = (_cachedCredential.Token.Scope ?? string.Empty)
            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        var missing = requiredScopes.Where(s => !granted.Contains(s)).ToArray();
        if (missing.Any())
        {
            throw new Exception($"Missing required scopes: {string.Join(',', missing)}. Please re-authenticate via /api/auth/login and grant the requested permissions.");
        }
    }

    public void ClearCachedCredential()
    {
        _cachedCredential = null;
    }
}