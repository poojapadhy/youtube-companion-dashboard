import React from 'react';

export default function VideoPlayer({ videoId, title }) {
  if (!videoId) return null;

  const src = `https://www.youtube-nocookie.com/embed/${videoId}?rel=0`;

  return (
    <div className="video-player">
      <iframe
        title={title || 'YouTube Video'}
        width="100%"
        height="480"
        src={src}
        frameBorder="0"
        allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
        allowFullScreen
      />
    </div>
  );
}
