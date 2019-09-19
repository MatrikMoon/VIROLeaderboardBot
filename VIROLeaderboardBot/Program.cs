using System;
using VIROLeaderboardBot.Discord;
using VIROLeaderboardBot.Misc;

namespace VIROLeaderboardBot
{
    class Program
    {
        static void Main(string[] args)
        {
            //Load server config
            Config.LoadConfig();

            //Start Discord bot
#if DEBUG
            var serverName = "Beat Saber Testing Server";
#else
            var serverName = "VIRO MOVE";
#endif

#if DEBUG
            ulong scoreChannel = 624102490480312330; //"event-scores";
#else
            ulong scoreChannel = 588345302453583888; //"lets-chat";
#endif
            VIROBot.Start(serverName, scoreChannel);

            Console.ReadLine();
        }
    }
}
