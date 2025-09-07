namespace WrapticaDesktop.Properties
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Configuration.SettingsGenerator", "4.0.0.0")]
    [global::System.Configuration.SettingsProviderAttribute(typeof(global::System.Configuration.LocalFileSettingsProvider))]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase
    {
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));

        public static Settings Default
        {
            get
            {
                return defaultInstance;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string SpotifyAccessToken
        {
            get
            {
                return ((string)(this["SpotifyAccessToken"]));
            }
            set
            {
                this["SpotifyAccessToken"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string SupabaseUrl
        {
            get
            {
                return ((string)(this["SupabaseUrl"]));
            }
            set
            {
                this["SupabaseUrl"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string SupabaseKey
        {
            get
            {
                return ((string)(this["SupabaseKey"]));
            }
            set
            {
                this["SupabaseKey"] = value;
            }
        }
    }
}
