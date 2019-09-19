using System;
using System.IO;
using VIROLeaderboardBot.Misc.SimpleJSON;

namespace VIROLeaderboardBot.Misc
{
    class Config
    {
        [Flags]
        public enum ServerFlags
        {
            None = 0,
        }

        public static string BotToken { get; set; }
        public static string BetaBotToken { get; set; }
        public static ServerFlags Flags { get; set; }

        private static string ConfigLocation = $"{Environment.CurrentDirectory}/Config.txt";

        public static void LoadConfig()
        {
            if (File.Exists(ConfigLocation))
            {
                JSONNode node = JSON.Parse(File.ReadAllText(ConfigLocation));
                BotToken = node["BotToken"].Value;
                BetaBotToken = node["BetaBotToken"].Value;
                Flags = (ServerFlags)Convert.ToInt32(node["ServerFlags"].Value);
            }
            else
            {
                BotToken = "[ReleaseToken]";
                BetaBotToken = "[BetaToken]";
                Flags = 0;
                SaveConfig();
            }
        }

        public static void SaveConfig()
        {
            JSONNode node = new JSONObject();
            node["BotToken"] = BotToken;
            node["BetaBotToken"] = BetaBotToken;
            node["ServerFlags"] = (int)Flags;
            File.WriteAllText(ConfigLocation, node.ToString());
        }
    }
}
