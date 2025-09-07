import React from 'react';
import { SessionSummary as SessionSummaryType } from '../types/session';

interface SessionSummaryProps {
  summary: SessionSummaryType;
}

const SessionSummary: React.FC<SessionSummaryProps> = ({ summary }) => {
  const formatDuration = (duration: string | number) => {
    if (typeof duration === 'number') {
      // Convert milliseconds to readable format
      const hours = Math.floor(duration / (1000 * 60 * 60));
      const minutes = Math.floor((duration % (1000 * 60 * 60)) / (1000 * 60));
      const seconds = Math.floor((duration % (1000 * 60)) / 1000);
      
      if (hours > 0) {
        return `${hours}h ${minutes}m ${seconds}s`;
      } else if (minutes > 0) {
        return `${minutes}m ${seconds}s`;
      } else {
        return `${seconds}s`;
      }
    }
    return duration;
  };

  const formatPercentage = (value: number) => {
    return `${value.toFixed(1)}%`;
  };

  return (
    <div className="session-summary">
      {/* Overview Cards */}
      <div className="summary-card">
        <div className="summary-title">
          <span>📊</span>
          <span>Total Sessions</span>
        </div>
        <div className="summary-value">{summary.totalSessions}</div>
        <div className="summary-label">Sessions tracked</div>
      </div>

      <div className="summary-card">
        <div className="summary-title">
          <span>⏱️</span>
          <span>Total Time</span>
        </div>
        <div className="summary-value">{formatDuration(summary.totalDuration)}</div>
        <div className="summary-label">Time tracked</div>
      </div>

      <div className="summary-card">
        <div className="summary-title">
          <span>📈</span>
          <span>Average Session</span>
        </div>
        <div className="summary-value">{formatDuration(summary.averageSessionDuration)}</div>
        <div className="summary-label">Per session</div>
      </div>

      <div className="summary-card">
        <div className="summary-title">
          <span>🏆</span>
          <span>Most Productive</span>
        </div>
        <div className="summary-value">{summary.mostProductiveDay}</div>
        <div className="summary-label">Best day of week</div>
      </div>

      {/* Top Apps */}
      <div className="summary-card">
        <div className="summary-title">
          <span>💻</span>
          <span>Top Applications</span>
        </div>
        <div className="top-list">
          {summary.topApps.slice(0, 5).map((app, index) => (
            <div key={app.name} className="top-item">
              <div className="top-item-rank">#{index + 1}</div>
              <div className="top-item-content">
                <div className="top-item-name">{app.name}</div>
                <div className="top-item-stats">
                  <span className="top-item-usage">{formatDuration(app.usage)}</span>
                  <span className="top-item-percentage">({formatPercentage(app.percentage)})</span>
                </div>
              </div>
              <div className="top-item-bar">
                <div 
                  className="top-item-progress" 
                  style={{ width: `${Math.min(app.percentage, 100)}%` }}
                />
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Top Artists */}
      <div className="summary-card">
        <div className="summary-title">
          <span>🎵</span>
          <span>Top Artists</span>
        </div>
        <div className="top-list">
          {summary.topArtists.slice(0, 5).map((artist, index) => (
            <div key={artist.name} className="top-item">
              <div className="top-item-rank">#{index + 1}</div>
              <div className="top-item-content">
                <div className="top-item-name">{artist.name}</div>
                <div className="top-item-stats">
                  <span className="top-item-usage">{artist.playCount} plays</span>
                  <span className="top-item-percentage">({formatPercentage(artist.percentage)})</span>
                </div>
              </div>
              <div className="top-item-bar">
                <div 
                  className="top-item-progress" 
                  style={{ width: `${Math.min(artist.percentage, 100)}%` }}
                />
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Top Songs */}
      <div className="summary-card">
        <div className="summary-title">
          <span>🎶</span>
          <span>Top Songs</span>
        </div>
        <div className="top-list">
          {summary.topSongs.slice(0, 5).map((song, index) => (
            <div key={`${song.name}-${song.artist}`} className="top-item">
              <div className="top-item-rank">#{index + 1}</div>
              <div className="top-item-content">
                <div className="top-item-name">{song.name}</div>
                <div className="top-item-artist">by {song.artist}</div>
                <div className="top-item-stats">
                  <span className="top-item-usage">{song.playCount} plays</span>
                  <span className="top-item-percentage">({formatPercentage(song.percentage)})</span>
                </div>
              </div>
              <div className="top-item-bar">
                <div 
                  className="top-item-progress" 
                  style={{ width: `${Math.min(song.percentage, 100)}%` }}
                />
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Productivity Insights */}
      <div className="summary-card">
        <div className="summary-title">
          <span>💡</span>
          <span>Productivity Insights</span>
        </div>
        <div className="insights">
          <div className="insight-item">
            <div className="insight-icon">🕐</div>
            <div className="insight-content">
              <div className="insight-title">Peak Hours</div>
              <div className="insight-value">{summary.mostProductiveHour}:00 - {summary.mostProductiveHour + 1}:00</div>
            </div>
          </div>
          <div className="insight-item">
            <div className="insight-icon">📅</div>
            <div className="insight-content">
              <div className="insight-title">Best Day</div>
              <div className="insight-value">{summary.mostProductiveDay}</div>
            </div>
          </div>
          <div className="insight-item">
            <div className="insight-icon">⚡</div>
            <div className="insight-content">
              <div className="insight-title">Focus Level</div>
              <div className="insight-value">
                {summary.totalSessions > 10 ? 'High' : summary.totalSessions > 5 ? 'Medium' : 'Getting Started'}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default SessionSummary;