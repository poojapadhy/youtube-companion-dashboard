import React, { useEffect, useState } from 'react';
import './App.css';
import API_BASE_URL from './config';
import VideoPlayer from './components/VideoPlayer';
import Comments from './components/Comments';
import Notes from './components/Notes';

function App() {
  const [video, setVideo] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function load() {
      try {
        const res = await fetch(`${API_BASE_URL}/api/video/details`);
        if (!res.ok) throw new Error('Failed to fetch video details');
        const json = await res.json();
        setVideo(json);
      } catch (e) {
        console.error(e);
      } finally {
        setLoading(false);
      }
    }
    load();
  }, []);

  return (
    <div className="app-root">
      <header className="app-top">
        <h1 className="app-header">Youtube Companion Dashboard</h1>
      </header>

      <main className="app-main">
        <section className="video-pane">
          {loading && <div className="placeholder">Loading video...</div>}
          {!loading && video && (
            <>
              <VideoPlayer videoId={video.videoId || video.VideoId} title={video.title || video.Title} />
              <div className="video-meta">
                <h2>{video.title || video.Title}</h2>
                <p className="desc">{video.description || video.Description}</p>
                <div className="stats">
                  <span>👁️ {video.viewCount ?? video.ViewCount}</span>
                  <span>👍 {video.likeCount ?? video.LikeCount}</span>
                  <span>💬 {video.commentCount ?? video.CommentCount}</span>
                </div>
              </div>
            </>
          )}
          {!loading && !video && <div className="placeholder">No video found.</div>}
        </section>

        <aside className="side-pane">
          <Comments />
          <Notes />
        </aside>
      </main>
    </div>
  );
}

export default App;
