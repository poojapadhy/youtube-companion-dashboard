# YouTube Companion Dashboard

.NET 10 API + React UI that helps manage a single YouTube video (fetch details, update title/description, manage comments), store notes in MongoDB, and generate title suggestions via Groq AI.

## Architecture overview
- ASP.NET Core API handles OAuth, YouTube Data API interactions, AI inference, and persistence.
- React UI is a thin client consuming REST endpoints.
- OAuth tokens are obtained via Google Web OAuth flow and cached in-memory (development scope).
- MongoDB Atlas stores notes and event logs.
- Groq (Llama-3.3-70B) is used via OpenAI-compatible API for AI title suggestions.

## Tech stack
- Backend: .NET 10 (ASP.NET Core)
- Frontend: React (Create React App)

Quick links
- App startup: [backend/YoutubeCompanion.API/Program.cs](backend/YoutubeCompanion.API/Program.cs)
- YouTube OAuth: [`YoutubeCompanion.Infrastructure.YouTube.YouTubeAuthService`](backend/YoutubeCompanion.Infrastructure/YouTube/YouTubeAuthService.cs)
- Groq AI client: [`YoutubeCompanion.Infrastructure.AI.GroqAiService`](backend/YoutubeCompanion.Infrastructure/AI/GroqAiService.cs)
- Mongo context: [`YoutubeCompanion.Infrastructure.Mongo.MongoContext`](backend/YoutubeCompanion.Infrastructure/Mongo/MongoContext.cs)
- Event logger: [`EventLogger`](backend/YoutubeCompanion.Infrastructure/Logging/EventLogger.cs)
- Frontend entry: [frontend/youtube-companion-ui/src/index.js](frontend/youtube-companion-ui/src/index.js)
- Frontend main: [frontend/youtube-companion-ui/src/App.js](frontend/youtube-companion-ui/src/App.js)
- Default config: [backend/YoutubeCompanion.API/appsettings.json](backend/YoutubeCompanion.API/appsettings.json)

## Main features / endpointsMain API endpoints
- Auth: GET /api/auth/login, GET /api/auth/callback — [backend/YoutubeCompanion.API/Controllers/AuthController.cs](backend/YoutubeCompanion.API/Controllers/AuthController.cs)
- Video: GET /api/video/details, PUT /api/video/update — [backend/YoutubeCompanion.API/Controllers/VideoController.cs](backend/YoutubeCompanion.API/Controllers/VideoController.cs)
- Comments: POST /api/video/comment, POST /api/video/comment/reply, DELETE /api/video/comment/{id} — [backend/YoutubeCompanion.API/Controllers/VideoController.cs](backend/YoutubeCompanion.API/Controllers/VideoController.cs)
- AI suggestions: GET /api/ai/suggest-titles — [backend/YoutubeCompanion.API/Controllers/AiController.cs](backend/YoutubeCompanion.API/Controllers/AiController.cs)
- Notes (Mongo): POST /api/notes, GET /api/notes/search — [backend/YoutubeCompanion.API/Controllers/NotesController.cs](backend/YoutubeCompanion.API/Controllers/NotesController.cs)


## Important files & symbols
- App startup: [backend/YoutubeCompanion.API/Program.cs](backend/YoutubeCompanion.API/Program.cs)
- YouTube OAuth: [`YoutubeCompanion.Infrastructure.YouTube.YouTubeAuthService`](backend/YoutubeCompanion.Infrastructure/YouTube/YouTubeAuthService.cs)
- Groq AI client: [`YoutubeCompanion.Infrastructure.AI.GroqAiService`](backend/YoutubeCompanion.Infrastructure/AI/GroqAiService.cs)
- MongoDB context: [`YoutubeCompanion.Infrastructure.Mongo.MongoContext`](backend/YoutubeCompanion.Infrastructure/Mongo/MongoContext.cs)
- Event logger: [backend/YoutubeCompanion.Infrastructure/Logging/EventLogger.cs](backend/YoutubeCompanion.Infrastructure/Logging/EventLogger.cs)
- Controllers:
  - [backend/YoutubeCompanion.API/Controllers/AuthController.cs](backend/YoutubeCompanion.API/Controllers/AuthController.cs)
  - [backend/YoutubeCompanion.API/Controllers/VideoController.cs](backend/YoutubeCompanion.API/Controllers/VideoController.cs)
  - [backend/YoutubeCompanion.API/Controllers/AiController.cs](backend/YoutubeCompanion.API/Controllers/AiController.cs)
  - [backend/YoutubeCompanion.API/Controllers/NotesController.cs](backend/YoutubeCompanion.API/Controllers/NotesController.cs)
- DTOs: [backend/YoutubeCompanion.Application/DTOs](backend/YoutubeCompanion.Application/DTOs)
- Frontend: [frontend/youtube-companion-ui/README.md](frontend/youtube-companion-ui/README.md), [frontend/youtube-companion-ui/package.json](frontend/youtube-companion-ui/package.json)

## Configuration
- Default app settings: [backend/YoutubeCompanion.API/appsettings.json](backend/YoutubeCompanion.API/appsettings.json)
  - YouTube keys: YouTube:ClientId, YouTube:ClientSecret, YouTube:RedirectUri, YouTube:VideoId
  - Mongo: MongoDb:ConnectionString, MongoDb:DatabaseName, MongoDb:NotesCollection
  - Groq: Groq:ApiKey, Groq:Model
- Development launch settings: [backend/YoutubeCompanion.API/Properties/launchSettings.json](backend/YoutubeCompanion.API/Properties/launchSettings.json)
- Keep sensitive values out of source control (use `appsettings.Development.json` or environment variables).

### Environment Variables (Production & Remote Deployment)
Set these environment variables for production or when running without a local `appsettings.Development.json`:

```bash
export YOUTUBE_CLIENT_ID="your_youtube_client_id"
export YOUTUBE_CLIENT_SECRET="your_youtube_client_secret"
export YOUTUBE_REDIRECT_URI="https://localhost:7009/api/auth/callback"
export YOUTUBE_VIDEO_ID="your_video_id"
export MONGODB_CONNECTION_STRING="your_mongodb_connection_string"
export GROQ_API_KEY="your_groq_api_key"
```

**Windows (PowerShell):**
```powershell
$env:YOUTUBE_CLIENT_ID = "your_youtube_client_id"
$env:YOUTUBE_CLIENT_SECRET = "your_youtube_client_secret"
$env:YOUTUBE_REDIRECT_URI = "https://localhost:7009/api/auth/callback"
$env:YOUTUBE_VIDEO_ID = "your_video_id"
$env:MONGODB_CONNECTION_STRING = "your_mongodb_connection_string"
$env:GROQ_API_KEY = "your_groq_api_key"
```

**Development (Local):**
For local development, use `appsettings.Development.json` (excluded from source control) — the app will automatically use it in Development environment.

## Run locally
- Backend
  - From backend folder: dotnet run (or open solution [backend/YoutubeCompanion.slnx](backend/YoutubeCompanion.slnx))
  - Ensure configuration (client secrets, Groq key, Mongo URI) available to the app.
- Frontend
  - From frontend/youtube-companion-ui: npm install && npm start
  - Entry: [frontend/youtube-companion-ui/src/index.js](frontend/youtube-companion-ui/src/index.js)

## Notes / gotchas
- OAuth write actions (comments, updates) require the `youtube.force-ssl` scope.  
  The application explicitly requests `YouTubeService.Scope.YoutubeForceSsl`; changing scopes requires re-consent.
- Credentials are cached in-memory by [`YoutubeCompanion.Infrastructure.YouTube.YouTubeAuthService.CacheCredential`](backend/YoutubeCompanion.Infrastructure/YouTube/YouTubeAuthService.cs) — process restart clears them.
- Groq AI responses are parsed naively (expects 3 lines); see [`GroqAiService.SuggestTitlesAsync`](backend/YoutubeCompanion.Infrastructure/AI/GroqAiService.cs).

## Licence
Refer to repository LICENSE (not present).