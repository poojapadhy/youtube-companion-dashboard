import React, { useEffect, useState } from 'react';
import API_BASE_URL from '../config';

export default function Notes() {
  const [notes, setNotes] = useState([]);
  const [content, setContent] = useState('');
  const [tags, setTags] = useState('');

  useEffect(() => {
    async function load() {
      try {
        const res = await fetch(`${API_BASE_URL}/api/notes/search`);
        if (!res.ok) return;
        const json = await res.json();
        setNotes(json);
      } catch (e) {
        console.error(e);
      }
    }
    load();
  }, []);

  async function addNote() {
    if (!content.trim()) return;
    try {
      const body = { content, tags: tags.split(',').map(t => t.trim()).filter(Boolean) };
      const res = await fetch(`${API_BASE_URL}/api/notes`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body),
        credentials: "include"
      });
      const json = await res.json();
      setNotes([json, ...notes]);
      setContent(''); setTags('');
    } catch (e) {
      console.error(e);
    }
  }

  return (
    <div className="notes-root">
      <h3>Notes</h3>
      <div className="add-note">
        <textarea placeholder="Quick note" value={content} onChange={e => setContent(e.target.value)} />
        <input placeholder="tags,comma,separated" value={tags} onChange={e => setTags(e.target.value)} />
        <button onClick={addNote}>Save</button>
      </div>
      <div className="notes-list">
        {notes.length === 0 && <div className="muted">No notes yet.</div>}
        {notes.map(n => (
          <div key={n.id || n._id || n.content} className="note-item">
            <div className="note-content">{n.content}</div>
            <div className="note-tags">{(n.tags||[]).join(', ')}</div>
          </div>
        ))}
      </div>
    </div>
  );
}
