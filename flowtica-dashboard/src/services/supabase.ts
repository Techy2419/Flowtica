import { createClient } from '@supabase/supabase-js';

const supabaseUrl = process.env.REACT_APP_SUPABASE_URL || 'https://your-project.supabase.co';
const supabaseKey = process.env.REACT_APP_SUPABASE_ANON_KEY || 'your-anon-key';

export const supabase = createClient(supabaseUrl, supabaseKey);

export interface Database {
  public: {
    Tables: {
      sessions: {
        Row: {
          id: string;
          session_id: string;
          start_time: string;
          end_time?: string;
          session_name: string;
          app_usages: any[];
          spotify_tracks: any[];
          created_at: string;
          updated_at: string;
        };
        Insert: {
          id?: string;
          session_id: string;
          start_time: string;
          end_time?: string;
          session_name: string;
          app_usages: any[];
          spotify_tracks: any[];
          created_at?: string;
          updated_at?: string;
        };
        Update: {
          id?: string;
          session_id?: string;
          start_time?: string;
          end_time?: string;
          session_name?: string;
          app_usages?: any[];
          spotify_tracks?: any[];
          created_at?: string;
          updated_at?: string;
        };
      };
    };
  };
}
