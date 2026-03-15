# YouTube Companion Dashboard

A full-stack tool to manage a single YouTube video — fetch details, update title/description, manage comments, store personal notes, and generate AI-powered title suggestions.

**Live app:** https://youtube-companion-dashboard-five.vercel.app  
**Backend:** ASP.NET Core (.NET 10) · **Frontend:** React 19 · **Database:** MongoDB Atlas · **AI:** Groq (Llama-3.3-70B)

---

## Features

- **Google OAuth 2.0** — secure login with YouTube scope-based access control (`youtube.force-ssl` for write operations)
- **Video management** — fetch video details, update title and description via YouTube Data API v3
- **Comment management** — post comments, reply to comments, delete comments
- **Notes** — store and search personal notes against the video, persisted in MongoDB Atlas
- **AI title suggestions** — generate 3 alternative video titles using Groq (Llama-3.3-70B) via OpenAI-compatible API
- **Event logging** — internal event logger for tracking API interactions
- **Swagger UI** — available in development (or when `ENABLE_SWAGGER=true`)

---

## Architecture

```
youtube-companion-dashboard/
├── backend/
│   └── YoutubeCompanion.API/           # ASP.NET Core Web API
│       ├── Controllers/                # Auth, Video, AI, Notes
│       ├── Properties/
│       └── appsettings.json
│   └── YoutubeCompanion.Application/
│       └── DTOs/                       # Request/response models
│   └── YoutubeCompanion.Infrastructure/
│       ├── AI/                         # Groq AI service
│       ├── Logging/                    # Event logger
│       ├── Mongo/                      # MongoDB context
│       ├── Settings/                   # Typed config classes
│       └── YouTube/                    # OAuth + YouTube API service
└── frontend/
    └── youtube-companion-ui/           # React 19 (Create React App)
        └── src/
```

The backend follows a layered architecture: **Controllers → Application (DTOs) → Infrastructure (services)**. The React frontend is a thin client consuming the REST API.

---

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/auth/login` | Initiate Google OAuth flow |
| GET | `/api/auth/callback` | OAuth callback handler |
| GET | `/api/video/details` | Fetch video metadata |
| PUT | `/api/video/update` | Update video title/description |
| POST | `/api/video/comment` | Post a comment |
| POST | `/api/video/comment/reply` | Reply to a comment |
| DELETE | `/api/video/comment/{id}` | Delete a comment |
| GET | `/api/ai/suggest-titles` | Get 3 AI-generated title suggestions |
| POST | `/api/notes` | Save a note |
| GET | `/api/notes/search` | Search notes |
| GET | `/api/config-check` | Verify environment config is loaded (non-secret) |

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)
- [MongoDB Atlas](https://www.mongodb.com/atlas) account (free tier works)
- [Google Cloud Console](https://console.cloud.google.com/) project with YouTube Data API v3 enabled
- [Groq API key](https://console.groq.com/)

---

### 1. Clone the repo

```bash
git clone https://github.com/poojapadhy/youtube-companion-dashboard.git
cd youtube-companion-dashboard
```

---

### 2. Configure the backend

Create `backend/YoutubeCompanion.API/appsettings.Development.json` (this file is git-ignored and never committed):

```json
{
  "YouTube": {
    "ClientId": "your_google_client_id",
    "ClientSecret": "your_google_client_secret",
    "RedirectUri": "https://localhost:7009/api/auth/callback",
    "VideoId": "your_youtube_video_id"
  },
  "MongoDb": {
    "ConnectionString": "your_mongodb_atlas_connection_string",
    "DatabaseName": "youtube_companion_db",
    "NotesCollection": "notes"
  },
  "Groq": {
    "ApiKey": "your_groq_api_key",
    "Model": "llama-3.3-70b-versatile"
  }
}
```

> **How to get these values:**
> - **YouTube ClientId / ClientSecret** — Create OAuth 2.0 credentials in [Google Cloud Console](https://console.cloud.google.com/apis/credentials). Add `https://localhost:7009/api/auth/callback` as an authorised redirect URI.
> - **VideoId** — The 11-character ID from any YouTube video URL: `youtube.com/watch?v=VIDEO_ID_HERE`
> - **MongoDB ConnectionString** — From your Atlas cluster under *Connect → Drivers*
> - **Groq ApiKey** — From [console.groq.com](https://console.groq.com/)

---

### 3. Run the backend

```bash
cd backend/YoutubeCompanion.API
dotnet run
```

The API starts at `https://localhost:7009`. Swagger UI is available at `https://localhost:7009/swagger`.

---

### 4. Run the frontend

```bash
cd frontend/youtube-companion-ui
npm install
npm start
```

The React app starts at `http://localhost:3000`.

---

## Production / Deployment

For production deployments (e.g. Render, Railway, Azure), set the following environment variables instead of using `appsettings.Development.json`:

| Environment Variable | Description |
|---|---|
| `YouTube__ClientId` | Google OAuth Client ID |
| `YouTube__ClientSecret` | Google OAuth Client Secret |
| `YouTube__RedirectUri` | Authorised OAuth redirect URI |
| `YouTube__VideoId` | Target YouTube video ID |
| `MongoDb__ConnectionString` | MongoDB Atlas connection string |
| `Groq__ApiKey` | Groq API key |
| `ENABLE_SWAGGER` | Set to `true` to expose Swagger in production |

> ASP.NET Core maps double-underscore (`__`) env vars to nested config keys automatically.

---

## Known Limitations & Planned Improvements

| Limitation | Planned Fix |
|---|---|
| OAuth tokens cached in-memory — lost on process restart | Persist encrypted tokens to MongoDB |
| AI title parser expects exactly 3 newline-separated lines | Switch to structured JSON response format from Groq |
| Hardcoded to manage a single video (configured via `VideoId`) | Allow dynamic video selection via UI |
| Frontend scaffolded with Create React App (deprecated) | Migrate to Vite |

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | .NET 10, ASP.NET Core |
| Frontend | React 19, Axios |
| Database | MongoDB Atlas |
| Auth | Google OAuth 2.0 (YouTube Data API v3) |
| AI | Groq API — Llama-3.3-70B |
| CI/Deploy | Vercel (frontend) |

---

## License

MIT License — Copyright (c) 2026 Pooja Padhy. See [LICENSE](./LICENSE) for details.