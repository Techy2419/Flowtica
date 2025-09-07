import { SessionData, SessionSummary } from '../types/session';

export class SessionService {
  static async getSessions(): Promise<SessionData[]> {
    try {
      const supabaseUrl = process.env.REACT_APP_SUPABASE_URL;
      const supabaseKey = process.env.REACT_APP_SUPABASE_ANON_KEY;
      
      if (!supabaseUrl || !supabaseKey) {
        console.warn('Supabase not configured. Please set REACT_APP_SUPABASE_URL and REACT_APP_SUPABASE_ANON_KEY');
        return [];
      }

      const response = await fetch(`${supabaseUrl}/rest/v1/sessions?select=*&order=start_time.desc`, {
        headers: {
          'apikey': supabaseKey,
          'Authorization': `Bearer ${supabaseKey}`,
          'Content-Type': 'application/json'
        }
      });

      if (response.ok) {
        const data = await response.json();
        return data.map(this.mapToSessionData);
      } else {
        console.error('Failed to fetch sessions from Supabase:', response.status, response.statusText);
        return [];
      }
    } catch (error) {
      console.error('Error fetching sessions:', error);
      return [];
    }
  }

  static async getSession(sessionId: string): Promise<SessionData | null> {
    try {
      const supabaseUrl = process.env.REACT_APP_SUPABASE_URL;
      const supabaseKey = process.env.REACT_APP_SUPABASE_ANON_KEY;
      
      if (supabaseUrl && supabaseKey) {
        const response = await fetch(`${supabaseUrl}/rest/v1/sessions?session_id=eq.${sessionId}&select=*`, {
          headers: {
            'apikey': supabaseKey,
            'Authorization': `Bearer ${supabaseKey}`,
            'Content-Type': 'application/json'
          }
        });

        if (response.ok) {
          const data = await response.json();
          return data.length > 0 ? this.mapToSessionData(data[0]) : null;
        }
      }

      return null;
    } catch (error) {
      console.error('Error fetching session:', error);
      return null;
    }
  }

  static async createSession(session: SessionData): Promise<boolean> {
    try {
      const supabaseUrl = process.env.REACT_APP_SUPABASE_URL;
      const supabaseKey = process.env.REACT_APP_SUPABASE_ANON_KEY;
      
      if (supabaseUrl && supabaseKey) {
        const response = await fetch(`${supabaseUrl}/rest/v1/sessions`, {
          method: 'POST',
          headers: {
            'apikey': supabaseKey,
            'Authorization': `Bearer ${supabaseKey}`,
            'Content-Type': 'application/json',
            'Prefer': 'return=minimal'
          },
          body: JSON.stringify({
            session_id: session.sessionId,
            start_time: session.startTime,
            end_time: session.endTime,
            session_name: session.sessionName,
            app_usages: session.appUsages,
            spotify_tracks: session.spotifyTracks
          })
        });

        return response.ok;
      }

      return false;
    } catch (error) {
      console.error('Error creating session:', error);
      return false;
    }
  }

  static async updateSession(session: SessionData): Promise<boolean> {
    try {
      const supabaseUrl = process.env.REACT_APP_SUPABASE_URL;
      const supabaseKey = process.env.REACT_APP_SUPABASE_ANON_KEY;
      
      if (supabaseUrl && supabaseKey) {
        const response = await fetch(`${supabaseUrl}/rest/v1/sessions?session_id=eq.${session.sessionId}`, {
          method: 'PATCH',
          headers: {
            'apikey': supabaseKey,
            'Authorization': `Bearer ${supabaseKey}`,
            'Content-Type': 'application/json',
            'Prefer': 'return=minimal'
          },
          body: JSON.stringify({
            end_time: session.endTime,
            session_name: session.sessionName,
            app_usages: session.appUsages,
            spotify_tracks: session.spotifyTracks,
            updated_at: new Date().toISOString()
          })
        });

        return response.ok;
      }

      return false;
    } catch (error) {
      console.error('Error updating session:', error);
      return false;
    }
  }

  static async deleteSession(sessionId: string): Promise<boolean> {
    try {
      const supabaseUrl = process.env.REACT_APP_SUPABASE_URL;
      const supabaseKey = process.env.REACT_APP_SUPABASE_ANON_KEY;
      
      if (supabaseUrl && supabaseKey) {
        const response = await fetch(`${supabaseUrl}/rest/v1/sessions?session_id=eq.${sessionId}`, {
          method: 'DELETE',
          headers: {
            'apikey': supabaseKey,
            'Authorization': `Bearer ${supabaseKey}`,
            'Content-Type': 'application/json',
            'Prefer': 'return=minimal'
          }
        });

        return response.ok;
      }

      return false;
    } catch (error) {
      console.error('Error deleting session:', error);
      return false;
    }
  }

  static async getSessionSummary(sessions: SessionData[]): Promise<SessionSummary> {
    const totalSessions = sessions.length;
    
    // Calculate total duration
    const totalDurationMs = sessions.reduce((total, session) => {
      const start = new Date(session.startTime);
      const end = session.endTime ? new Date(session.endTime) : new Date();
      return total + (end.getTime() - start.getTime());
    }, 0);
    
    const totalDuration = this.formatDuration(totalDurationMs);

    // Calculate top apps
    const appUsageMap = new Map<string, number>();
    sessions.forEach(session => {
      session.appUsages.forEach(app => {
        const current = appUsageMap.get(app.appName) || 0;
        appUsageMap.set(app.appName, current + this.parseDuration(app.duration));
      });
    });

    const totalAppTime = Array.from(appUsageMap.values()).reduce((sum, time) => sum + time, 0);
    const topApps = Array.from(appUsageMap.entries())
      .map(([name, usage]) => ({
        name,
        usage,
        percentage: totalAppTime > 0 ? (usage / totalAppTime) * 100 : 0
      }))
      .sort((a, b) => b.usage - a.usage)
      .slice(0, 10);

    // Calculate top artists
    const artistMap = new Map<string, number>();
    sessions.forEach(session => {
      session.spotifyTracks.forEach(track => {
        if (track.artistName) {
          const current = artistMap.get(track.artistName) || 0;
          artistMap.set(track.artistName, current + 1);
        }
      });
    });

    const totalTracks = Array.from(artistMap.values()).reduce((sum, count) => sum + count, 0);
    const topArtists = Array.from(artistMap.entries())
      .map(([name, playCount]) => ({
        name,
        playCount,
        percentage: totalTracks > 0 ? (playCount / totalTracks) * 100 : 0
      }))
      .sort((a, b) => b.playCount - a.playCount)
      .slice(0, 10);

    // Calculate top songs
    const songMap = new Map<string, { artist: string; count: number }>();
    sessions.forEach(session => {
      session.spotifyTracks.forEach(track => {
        if (track.trackName && track.artistName) {
          const key = `${track.trackName} - ${track.artistName}`;
          const current = songMap.get(key) || { artist: track.artistName, count: 0 };
          songMap.set(key, { ...current, count: current.count + 1 });
        }
      });
    });

    const topSongs = Array.from(songMap.entries())
      .map(([name, data]) => ({
        name: name.split(' - ')[0],
        artist: data.artist,
        playCount: data.count,
        percentage: totalTracks > 0 ? (data.count / totalTracks) * 100 : 0
      }))
      .sort((a, b) => b.playCount - a.playCount)
      .slice(0, 10);

    // Calculate average session duration
    const averageDuration = totalSessions > 0 ? totalDurationMs / totalSessions : 0;
    const averageSessionDuration = this.formatDuration(averageDuration);

    // Find most productive day and hour
    const dayMap = new Map<string, number>();
    const hourMap = new Map<number, number>();
    
    sessions.forEach(session => {
      const start = new Date(session.startTime);
      const day = start.toLocaleDateString('en-US', { weekday: 'long' });
      const hour = start.getHours();
      
      const duration = session.endTime 
        ? new Date(session.endTime).getTime() - start.getTime()
        : Date.now() - start.getTime();
      
      dayMap.set(day, (dayMap.get(day) || 0) + duration);
      hourMap.set(hour, (hourMap.get(hour) || 0) + duration);
    });

    const mostProductiveDay = Array.from(dayMap.entries())
      .sort((a, b) => b[1] - a[1])[0]?.[0] || 'Unknown';
    
    const mostProductiveHour = Array.from(hourMap.entries())
      .sort((a, b) => b[1] - a[1])[0]?.[0] || 0;

    return {
      totalSessions,
      totalDuration,
      topApps,
      topArtists,
      topSongs,
      averageSessionDuration,
      mostProductiveDay,
      mostProductiveHour
    };
  }


  private static mapToSessionData(data: any): SessionData {
    return {
      sessionId: data.session_id,
      startTime: data.start_time,
      endTime: data.end_time,
      sessionName: data.session_name,
      appUsages: data.app_usages || [],
      spotifyTracks: data.spotify_tracks || [],
      isActive: !data.end_time
    };
  }

  private static formatDuration(ms: number): string {
    const hours = Math.floor(ms / (1000 * 60 * 60));
    const minutes = Math.floor((ms % (1000 * 60 * 60)) / (1000 * 60));
    const seconds = Math.floor((ms % (1000 * 60)) / 1000);
    
    if (hours > 0) {
      return `${hours}h ${minutes}m ${seconds}s`;
    } else if (minutes > 0) {
      return `${minutes}m ${seconds}s`;
    } else {
      return `${seconds}s`;
    }
  }

  private static parseDuration(duration: string): number {
    const parts = duration.split(':').map(Number);
    if (parts.length === 3) {
      return (parts[0] * 3600 + parts[1] * 60 + parts[2]) * 1000;
    }
    return 0;
  }
}