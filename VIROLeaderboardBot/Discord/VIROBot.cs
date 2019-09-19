using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using VIROLeaderboardBot.Discord.Services;
using VIROLeaderboardBot.Misc;

namespace VIROLeaderboardBot.Discord
{
    class VIROBot
    {
        private static DiscordSocketClient _client;
        private static IServiceProvider _services;
        private static string _serverName;
        private static ulong _scoreChannel;
        private static string _voteChannel;
        private static string _databaseLocation;

        public static void Start(string serverName, ulong scoreChannel, string voteChannel, string databaseLocation = "botDatabase.db")
        {
            _serverName = serverName;
            _scoreChannel = scoreChannel;
            _voteChannel = voteChannel;
            _databaseLocation = databaseLocation;
            MainAsync().GetAwaiter().GetResult();
        }

        public static void SendToScoreChannel(string message)
        {
            var guild = _client.Guilds.ToList().Where(x => x.Name.Contains(_serverName)).First();
            guild.GetTextChannel(_scoreChannel).SendMessageAsync(message);
        }

        public static async Task MainAsync()
        {
            _services = ConfigureServices();

            _client = _services.GetRequiredService<DiscordSocketClient>();

            _client.Log += LogAsync;
            _services.GetRequiredService<CommandService>().Log += LogAsync;

#if DEBUG
            await _client.LoginAsync(TokenType.Bot, Config.BetaBotToken);
#else
            await _client.LoginAsync(TokenType.Bot, Config.BotToken);
#endif
            await _client.StartAsync();
            await _services.GetRequiredService<CommandHandlingService>().InitializeAsync();

            //_services.GetRequiredService<LeaderboardService>().StartPolling(60 * 1000);
        }

        public static IServiceProvider GetServices() => _services;

        private static Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }

        private static IServiceProvider ConfigureServices()
        {
            var config = new DiscordSocketConfig { MessageCacheSize = 100 };
            return new ServiceCollection()
                .AddSingleton(new DiscordSocketClient(config))
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton(serviceProvider => new DatabaseService(_databaseLocation, serviceProvider))
                .BuildServiceProvider();
        }
    }
}
