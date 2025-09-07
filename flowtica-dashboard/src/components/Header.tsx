import React from 'react';

interface HeaderProps {
  onRefresh: () => void;
  lastUpdated?: Date;
}

const Header: React.FC<HeaderProps> = ({ onRefresh, lastUpdated }) => {
  const formatLastUpdated = (date: Date) => {
    const now = new Date();
    const diff = now.getTime() - date.getTime();
    const minutes = Math.floor(diff / 60000);
    
    if (minutes < 1) return 'Just now';
    if (minutes < 60) return `${minutes}m ago`;
    
    const hours = Math.floor(minutes / 60);
    if (hours < 24) return `${hours}h ago`;
    
    const days = Math.floor(hours / 24);
    return `${days}d ago`;
  };

  return (
    <header className="header">
      <div className="header-content">
        <div className="header-left">
          <h1 className="header-title">🚀 Flowtica Dashboard</h1>
          <p className="header-subtitle">Personal Productivity Tracker</p>
        </div>
        <div className="header-actions">
          {lastUpdated && (
            <div className="last-updated">
              <span>🕒</span>
              <span>Last updated: {formatLastUpdated(lastUpdated)}</span>
            </div>
          )}
          <button className="refresh-btn" onClick={onRefresh}>
            <span>🔄</span>
            <span>Refresh</span>
          </button>
        </div>
      </div>
    </header>
  );
};

export default Header;