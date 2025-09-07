-- Create sessions table
CREATE TABLE IF NOT EXISTS sessions (
    id UUID DEFAULT gen_random_uuid() PRIMARY KEY,
    session_id TEXT NOT NULL UNIQUE,
    start_time TIMESTAMPTZ NOT NULL,
    end_time TIMESTAMPTZ,
    session_name TEXT NOT NULL DEFAULT 'Focus Session',
    app_usages JSONB DEFAULT '[]'::jsonb,
    spotify_tracks JSONB DEFAULT '[]'::jsonb,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_sessions_start_time ON sessions(start_time DESC);
CREATE INDEX IF NOT EXISTS idx_sessions_session_id ON sessions(session_id);
CREATE INDEX IF NOT EXISTS idx_sessions_created_at ON sessions(created_at DESC);

-- Create function to update updated_at timestamp
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Create trigger to automatically update updated_at
CREATE TRIGGER update_sessions_updated_at 
    BEFORE UPDATE ON sessions 
    FOR EACH ROW 
    EXECUTE FUNCTION update_updated_at_column();

-- Enable Row Level Security (RLS)
ALTER TABLE sessions ENABLE ROW LEVEL SECURITY;

-- Create policy to allow all operations for now (single-user prototype)
-- In Phase 3, this will be updated to use user authentication
CREATE POLICY "Allow all operations on sessions" ON sessions
    FOR ALL USING (true);

-- Create a view for session statistics
CREATE OR REPLACE VIEW session_stats AS
SELECT 
    COUNT(*) as total_sessions,
    SUM(EXTRACT(EPOCH FROM (COALESCE(end_time, NOW()) - start_time))) as total_duration_seconds,
    AVG(EXTRACT(EPOCH FROM (COALESCE(end_time, NOW()) - start_time))) as avg_duration_seconds,
    COUNT(CASE WHEN end_time IS NULL THEN 1 END) as active_sessions,
    DATE_TRUNC('day', start_time) as session_date,
    EXTRACT(HOUR FROM start_time) as session_hour
FROM sessions
GROUP BY DATE_TRUNC('day', start_time), EXTRACT(HOUR FROM start_time)
ORDER BY session_date DESC, session_hour DESC;

-- Create a function to get top apps from a session
CREATE OR REPLACE FUNCTION get_top_apps(session_uuid UUID)
RETURNS TABLE(app_name TEXT, usage_count BIGINT) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        (app_usage->>'appName')::TEXT as app_name,
        COUNT(*) as usage_count
    FROM sessions s,
         jsonb_array_elements(s.app_usages) as app_usage
    WHERE s.id = session_uuid
    GROUP BY (app_usage->>'appName')::TEXT
    ORDER BY usage_count DESC
    LIMIT 10;
END;
$$ LANGUAGE plpgsql;

-- Create a function to get top artists from a session
CREATE OR REPLACE FUNCTION get_top_artists(session_uuid UUID)
RETURNS TABLE(artist_name TEXT, play_count BIGINT) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        (spotify_track->>'artistName')::TEXT as artist_name,
        COUNT(*) as play_count
    FROM sessions s,
         jsonb_array_elements(s.spotify_tracks) as spotify_track
    WHERE s.id = session_uuid
      AND (spotify_track->>'artistName')::TEXT IS NOT NULL
      AND (spotify_track->>'artistName')::TEXT != ''
    GROUP BY (spotify_track->>'artistName')::TEXT
    ORDER BY play_count DESC
    LIMIT 10;
END;
$$ LANGUAGE plpgsql;
