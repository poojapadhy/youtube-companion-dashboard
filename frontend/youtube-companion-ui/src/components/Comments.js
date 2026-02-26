import React, { useState } from 'react';
import API_BASE_URL from '../config';

function CommentItem({ c, onReply, onDelete }) {
  const [replying, setReplying] = useState(false);
  const [replyText, setReplyText] = useState('');

  return (
    <div className="comment-item">
      <div className="comment-text">{c.text}</div>
      <div className="comment-actions">
        <button onClick={() => setReplying(r => !r)}>Reply</button>
        {c.id && <button onClick={() => onDelete && onDelete(c.id)}>Delete</button>}
      </div>
      {replying && (
        <div className="reply-box">
          <input value={replyText} onChange={e => setReplyText(e.target.value)} placeholder="Write a reply" />
          <button onClick={() => { onReply && onReply(c.id, replyText); setReplyText(''); setReplying(false); }}>Send</button>
        </div>
      )}
      {c.replies && c.replies.length > 0 && (
        <div className="replies">
          {c.replies.map(r => (
            <div key={r.id} className="reply">{r.text}</div>
          ))}
        </div>
      )}
    </div>
  );
}

export default function Comments() {
  const [comments, setComments] = useState([]);
  const [text, setText] = useState('');
  const [loading, setLoading] = useState(false);

  async function addComment() {
    if (!text.trim()) return;
    setLoading(true);
    try {
      const res = await fetch(`${API_BASE_URL}/api/video/comment`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ text })
      });
      const json = await res.json();
      const id = json.commentId ?? json.commentId;
      const newComment = { id, text, replies: [] };
      setComments([newComment, ...comments]);
      setText('');
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  }

  async function replyTo(parentId, replyText) {
    if (!replyText || !parentId) return;
    try {
      const res = await fetch(`${API_BASE_URL}/api/video/comment/reply`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ parentCommentId: parentId, text: replyText })
      });
      const json = await res.json();
      const replyId = json.replyId;
      setComments(cs => cs.map(c => c.id === parentId ? { ...c, replies: [...(c.replies||[]), { id: replyId, text: replyText }] } : c));
    } catch (e) {
      console.error(e);
    }
  }

  async function deleteComment(id) {
    try {
      await fetch(`${API_BASE_URL}/api/video/comment/${id}`, { method: 'DELETE' });
      setComments(cs => cs.filter(c => c.id !== id));
    } catch (e) {
      console.error(e);
    }
  }

  return (
    <div className="comments-root">
      <h3>Comments</h3>
      <div className="add-comment">
        <input placeholder="Add a comment" value={text} onChange={e => setText(e.target.value)} />
        <button onClick={addComment} disabled={loading}>Post</button>
      </div>
      <div className="comments-list">
        {comments.length === 0 && <div className="muted">No comments yet. Add one!</div>}
        {comments.map(c => (
          <CommentItem key={c.id || c.text} c={c} onReply={replyTo} onDelete={deleteComment} />
        ))}
      </div>
    </div>
  );
}
