import React, { useState, useEffect, useCallback } from 'react';
import { SessionData, SessionSummary } from './types/session';
import { SessionService } from './services/sessionService';
import Header from './components/Header';
import SessionList from './components/SessionList';
import SessionSummaryComponent from './components/SessionSummary';
import LoadingSpinner from './components/LoadingSpinner';
import './App.css';

function App() {
  const [sessions, setSessions] = useState<SessionData[]>([]);
  const [summary, setSummary] = useState<SessionSummary | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [lastUpdated, setLastUpdated] = useState<Date>(new Date());

  const fetchSessions = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const fetchedSessions = await SessionService.getSessions();
      setSessions(fetchedSessions);
      
      // Calculate summary
      if (fetchedSessions.length > 0) {
        const sessionSummary = await SessionService.getSessionSummary(fetchedSessions);
        setSummary(sessionSummary);
      } else {
        setSummary(null);
      }
      
      setLastUpdated(new Date());
    } catch (err) {
      console.error('Failed to fetch sessions:', err);
      setError('Failed to load sessions. Please ensure Supabase is configured and the desktop app is running.');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchSessions();
    
    // Set up auto-refresh every 30 seconds
    const interval = setInterval(fetchSessions, 30000);
    return () => clearInterval(interval);
  }, [fetchSessions]);

  const handleDeleteSession = async (sessionId: string) => {
    try {
      const success = await SessionService.deleteSession(sessionId);
      if (success) {
        await fetchSessions(); // Refresh sessions after deletion
      } else {
        setError('Failed to delete session. Please try again.');
      }
    } catch (err) {
      console.error('Failed to delete session:', err);
      setError('Failed to delete session.');
    }
  };

  const handleRefresh = () => {
    fetchSessions();
  };

  return (
    <div className="app">
      <Header onRefresh={handleRefresh} lastUpdated={lastUpdated} />
      <div className="main-content">
        {loading && <LoadingSpinner />}
        {error && (
          <div className="error-banner">
            <div className="error-content">
              <span className="error-icon">⚠️</span>
              <span className="error-message">{error}</span>
              <button className="error-dismiss" onClick={() => setError(null)}>×</button>
            </div>
          </div>
        )}
        {!loading && !error && sessions.length === 0 && (
          <div className="empty-state">
            <div className="empty-icon">📊</div>
            <h2>No sessions found</h2>
            <p>Start tracking your productivity with the Flowtica desktop app!</p>
            <div className="empty-instructions">
              <h3>Getting Started:</h3>
              <ol>
                <li>Download and run the Flowtica desktop application</li>
                <li>Configure your Spotify and Supabase credentials in Settings</li>
                <li>Start a session to begin tracking your productivity</li>
                <li>View your analytics here in real-time</li>
              </ol>
            </div>
            <div className="empty-actions">
              <button className="btn-primary" onClick={handleRefresh}>
                🔄 Refresh
              </button>
            </div>
          </div>
        )}
        {summary && <SessionSummaryComponent summary={summary} />}
        <SessionList 
          sessions={sessions} 
          onDeleteSession={handleDeleteSession}
        />
      </div>
    </div>
  );
}

export default App;