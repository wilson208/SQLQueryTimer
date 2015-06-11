using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLQueryTimer.Model;
using System.Windows;
using Newtonsoft.Json;

namespace SQLQueryTimer.Utilities
{
    public class SettingsUtility
    {
        public static Settings GetSettings()
        {
            if (!CheckSettingsFileExist())
                SetSettings(GetDefaultSettings());
            return ReadSettingsFromFile();
        }

        public static void SetSettings(Settings settings)
        {
            if (!Directory.Exists(GetSaveFolder()))
                Directory.CreateDirectory(GetSaveFolder());

            var json = JsonConvert.SerializeObject(settings);
            var filepath = Path.Combine(GetSaveFolder(), GetFileName());

            File.WriteAllText(filepath, json);
        }

        private static Settings ReadSettingsFromFile()
        {
            var filepath = Path.Combine(GetSaveFolder(), GetFileName());
            var json = File.ReadAllText(filepath);
            var settings = JsonConvert.DeserializeObject<Settings>(json);
            return settings;
        }

        private static Settings GetDefaultSettings()
        {
            return new Settings()
            {
                AlwaysOnTop = false,
                Queries = new List<Query>()
                {
                    new Query()
                    {
                        ConnectionString = "Data Source=uojs0m9frw.database.windows.net;Initial Catalog=TweetMiner_db;User ID=wilson;Password=Seam33.an",
                        IntervalMilliseconds = 5000,
                        Name = "Tweets in miner db",
                        SqlQuery = "SELECT COUNT(1) FROM dbo.Tweets"
                    }
                }
            };
        }
        private static bool CheckSettingsFileExist()
        {
            var filepath = Path.Combine(GetSaveFolder(), GetFileName());
            if (!Directory.Exists(GetSaveFolder()))
                return false;

            return File.Exists(filepath);
        }
        private static string GetSaveFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "SQLQueryTimer");
        }
        private static string GetFileName()
        {
            return "SQLQueryTime.settings";
        }
    }
}
