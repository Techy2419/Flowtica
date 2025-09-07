import React from 'react';
import { SessionData } from '../types/session';

interface SessionListProps {
  sessions: SessionData[];
  onDeleteSession: (sessionId: string) => void;
}

const SessionList: React.FC<SessionListProps> = ({ sessions, onDeleteSession }) => {
  const formatDuration = (startTime: string, endTime?: string) => {
    const start = new Date(startTime);
    const end = endTime ? new Date(endTime) : new Date();
    const diff = end.getTime() - start.getTime();
    
    const hours = Math.floor(diff / (1000 * 60 * 60));
    const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
    const seconds = Math.floor((diff % (1000 * 60)) / 1000);
    
    if (hours > 0) {
      return `${hours}h ${minutes}m ${seconds}s`;
    } else if (minutes > 0) {
      return `${minutes}m ${seconds}s`;
    } else {
      return `${seconds}s`;
    }
  };

  const formatDateTime = (dateString: string) => {
    const date = new Date(dateString);
    return {
      date: date.toLocaleDateString('en-US', { 
        weekday: 'short', 
        month: 'short', 
        day: 'numeric' 
      }),
      time: date.toLocaleTimeString('en-US', { 
        hour: '2-digit', 
        minute: '2-digit' 
      })
    };
  };

  const getProductivityScore = (session: SessionData) => {
    // Simple productivity score based on app usage patterns
    const productiveApps = ['Code', 'Visual Studio', 'Chrome', 'Firefox', 'Notepad++', 'Sublime Text', 'Atom', 'Word', 'Excel', 'PowerPoint'];
    const unproductiveApps = ['Steam', 'Discord', 'Telegram', 'WhatsApp', 'Facebook', 'Twitter', 'Instagram', 'TikTok', 'YouTube'];
    
    const productiveTime = session.appUsages
      .filter(app => productiveApps.some(pa => app.appName.toLowerCase().includes(pa.toLowerCase())))
      .reduce((total, app) => {
        const [hours, minutes, seconds] = app.duration.split(':').map(Number);
        return total + (hours * 3600 + minutes * 60 + seconds);
      }, 0);
    
    const unproductiveTime = session.appUsages
      .filter(app => unproductiveApps.some(ua => app.appName.toLowerCase().includes(ua.toLowerCase())))
      .reduce((total, app) => {
        const [hours, minutes, seconds] = app.duration.split(':').map(Number);
        return total + (hours * 3600 + minutes * 60 + seconds);
      }, 0);
    
    const totalTime = session.appUsages.reduce((total, app) => {
      const [hours, minutes, seconds] = app.duration.split(':').map(Number);
      return total + (hours * 3600 + minutes * 60 + seconds);
    }, 0);
    
    if (totalTime === 0) return 50;
    
    const score = ((productiveTime - unproductiveTime) / totalTime) * 100;
    return Math.max(0, Math.min(100, score + 50));
  };

  const getScoreColor = (score: number) => {
    if (score >= 80) return '#10b981';
    if (score >= 60) return '#f59e0b';
    return '#ef4444';
  };

  const getScoreLabel = (score: number) => {
    if (score >= 80) return 'Excellent';
    if (score >= 60) return 'Good';
    if (score >= 40) return 'Fair';
    return 'Needs Focus';
  };

  if (sessions.length === 0) {
    return (
      <div className="session-list">
        <div className="card-header">
          <h2 className="card-title">
            <span>📊</span>
            <span>Session History</span>
          </h2>
        </div>
        <div className="card-content">
          <div className="empty-state">
            <div className="empty-icon">📊</div>
            <h3>No sessions found</h3>
            <p>Start your first session with the Flowtica desktop app!</p>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="session-list">
      <div className="card-header">
        <h2 className="card-title">
          <span>📊</span>
          <span>Session History</span>
        </h2>
        <div className="session-count">
          {sessions.length} session{sessions.length !== 1 ? 's' : ''}
        </div>
      </div>
      <div className="card-content">
        {sessions.map((session) => {
          const { date, time } = formatDateTime(session.startTime);
          const duration = formatDuration(session.startTime, session.endTime);
          const productivityScore = getProductivityScore(session);
          const scoreColor = getScoreColor(productivityScore);
          const scoreLabel = getScoreLabel(productivityScore);
          
          return (
            <div key={session.sessionId} className="session-item fade-in">
              <div className="session-header">
                <h3 className="session-name">{session.sessionName}</h3>
                <div className={`session-status ${session.isActive ? 'active' : 'completed'}`}>
                  {session.isActive ? 'Active' : 'Completed'}
                </div>
              </div>
              
              <div className="session-meta">
                <div className="session-meta-item">
                  <div className="session-meta-label">Date</div>
                  <div className="session-meta-value">{date}</div>
                </div>
                <div className="session-meta-item">
                  <div className="session-meta-label">Start Time</div>
                  <div className="session-meta-value">{time}</div>
                </div>
                <div className="session-meta-item">
                  <div className="session-meta-label">Duration</div>
                  <div className="session-meta-value">{duration}</div>
                </div>
                <div className="session-meta-item">
                  <div className="session-meta-label">Productivity</div>
                  <div className="session-meta-value" style={{ color: scoreColor }}>
                    {productivityScore.toFixed(1)}% - {scoreLabel}
                  </div>
                </div>
              </div>
              
              <div className="session-stats">
                <div className="session-stat">
                  <span>💻</span>
                  <span>{session.appUsages.length} apps</span>
                </div>
                <div className="session-stat">
                  <span>🎵</span>
                  <span>{session.spotifyTracks.length} songs</span>
                </div>
                <div className="session-stat">
                  <span>⏱️</span>
                  <span>{duration}</span>
                </div>
              </div>
              
              <div className="session-actions">
                <button 
                  className="btn-danger"
                  onClick={() => onDeleteSession(session.sessionId)}
                >
                  <span>🗑️</span>
                  <span>Delete</span>
                </button>
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
};

export default SessionList;