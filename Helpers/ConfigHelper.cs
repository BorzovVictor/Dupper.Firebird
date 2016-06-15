using System;
using System.Collections.Specialized;
using System.Configuration;

namespace Dupper.Firebird.Helpers
{
  /* add into cofig file
  <appSettings>
    <add key = "datasource" value="192.168.1.2"/>
    <add key = "database" value="d:\base\test.FDB"/>
    <add key = "user" value="sysdba"/>
    <add key = "pass" value="masterkey"/>
  </appSettings>
  */
    public class ConfigHelper
    {
       static NameValueCollection appSettings = ConfigurationManager.AppSettings;
        private static string datasource;
        private static string database;
        private static string user;
        private static string pass;
        public static string ReadConnectionString()
        {
            try
            {

                if (appSettings.Count == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return
                        $"datasource={appSettings[nameof(datasource)]}; database={appSettings[nameof(database)]};userid={appSettings[nameof(user)]};password={appSettings[nameof(pass)]}";
                }
            }
            catch (ConfigurationErrorsException e)
            {
                throw;
            }
        }

        public static bool WriteSettings(string server, string db, string userId, string password)
        {
            try
            {
                UpdateKey(nameof(datasource), server);
                UpdateKey(nameof(database), db);
                UpdateKey(nameof(user), userId);
                UpdateKey(nameof(pass), password);
                return true;
            }
            catch (ConfigurationErrorsException)
            {
                return false;
            }
        }

        public static bool CheckSettings()
        {
            if (appSettings[nameof(datasource)] == null) return false;
            if (appSettings[nameof(database)] == null) return false;
            if (appSettings[nameof(user)] == null) return false;
            if (appSettings[nameof(pass)] == null) return false;
            return true;
        }

        private static void UpdateKey(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                throw;
            }
        }
    }
}
