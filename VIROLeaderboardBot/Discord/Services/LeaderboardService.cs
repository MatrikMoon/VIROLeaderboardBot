using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using VIROLeaderboardBot.Discord.Database;
using VIROLeaderboardBot.Misc.SimpleJSON;

namespace VIROLeaderboardBot.Discord.Services
{
    public class LeaderboardService
    {
        public class Standing
        {
            public int Ranking { get; set; }
            public string PlayerId { get; set; }
            public string PlayerName { get; set; }
            public float Points { get; set; }
        }

        public enum GameType
        {
            Overall,
            Boxing,
            Swording,
            Shooting,
            WeaponMastery
        }

        private readonly DatabaseService _databaseService;
        private readonly HttpClient _httpClient;
        private Timer _pollTimer = new Timer();

        public LeaderboardService(IServiceProvider provider)
        {
            _httpClient = provider.GetRequiredService<HttpClient>();
            _databaseService = provider.GetRequiredService<DatabaseService>();
        }

        public void StartPolling(long interval)
        {
            _pollTimer.Interval = interval;
            _pollTimer.Elapsed += pollTimer_Elapsed;
            _pollTimer.Start();
        }

        private async void pollTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await Poll();
        }

        public async Task Poll()
        {
            var changedBoxingScores = await ParseForLeaderboardChanges(GameType.Boxing);
            var changedSwordsScores = await ParseForLeaderboardChanges(GameType.Swording);
            var changedShootingScores = await ParseForLeaderboardChanges(GameType.Shooting);
            var changedWeaponMasterScores = await ParseForLeaderboardChanges(GameType.WeaponMastery);

            string reply = string.Empty;

            if (changedBoxingScores.Count > 0)
            {
                reply += "New Boxing Scores:\n";
                foreach (var score in changedBoxingScores) reply += $"{score.Ranking}:\t{score.PlayerName}\t({score.Points})\n";
            }
            if (changedSwordsScores.Count > 0)
            {
                reply += "\nNew Swords Scores:\n";
                foreach (var score in changedSwordsScores) reply += $"{score.Ranking}:\t{score.PlayerName}\t({score.Points})\n";
            }
            if (changedShootingScores.Count > 0)
            {
                reply += "\nNew Shooting Scores:\n";
                foreach (var score in changedShootingScores) reply += $"{score.Ranking}:\t{score.PlayerName}\t({score.Points})\n";
            }
            if (changedWeaponMasterScores.Count > 0)
            {
                reply += "\nNew Weapon Master Scores:\n";
                foreach (var score in changedWeaponMasterScores) reply += $"{score.Ranking}:\t{score.PlayerName}\t({score.Points})\n";
            }

            //Deal with long messages
            if (reply.Length > 2000)
            {
                for (int i = 0; reply.Length > 2000; i++)
                {
                    await VIROBot.SendToScoreChannel(reply.Substring(0, reply.Length > 2000 ? 2000 : reply.Length));
                    reply = reply.Substring(2000);
                }
            }
            if (reply.Length > 0) await VIROBot.SendToScoreChannel(reply);
        }

        public async Task<List<Score>> ParseForLeaderboardChanges(GameType gameType)
        {
            var array = await GetLeaderboardArrayFromApi(gameType);

            List<Score> changedScores = new List<Score>();

            foreach (var item in array)
            {
                var score = new Score()
                {
                    Ranking = item.Value["ranking"].AsInt,
                    PlayerName = item.Value["gamerNickname"],
                    PlayerId = item.Value["gamerGuid"],
                    Points = item.Value["points"].AsInt,
                    GameType = (int)gameType,
                    Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds()
                };

                var scores = _databaseService.DatabaseContext.Scores;

                //If this is a new score, add it to the database.
                //We'll judge new-ness by "if it's a different number score for the player"
                if (scores.Any(x => x.PlayerId == score.PlayerId && x.GameType == score.GameType))
                {
                    var oldScores = scores.Where(x => x.PlayerId == score.PlayerId && x.GameType == score.GameType);
                    if (!oldScores.Any(x => x.Points == score.Points))
                    {
                        foreach (var oldScore in oldScores) oldScore.Old = true;
                        changedScores.Add(score);
                        scores.Add(score);
                        await _databaseService.DatabaseContext.SaveChangesAsync();
                    }
                }
                else
                {
                    changedScores.Add(score);
                    scores.Add(score);
                    await _databaseService.DatabaseContext.SaveChangesAsync();
                }
            }

            return changedScores;
        }

        public async Task<List<Standing>> GetOverallStandings()
        {
            var array = await  GetLeaderboardArrayFromApi(GameType.Overall);
            List<Standing> standings = new List<Standing>();

            foreach (var item in array)
            {
                var standing = new Standing
                {
                    Ranking = item.Value["ranking"].AsInt,
                    PlayerId = item.Value["gamerGuid"],
                    PlayerName = item.Value["gamerNickname"],
                    Points = item.Value["points"].AsFloat
                };

                standings.Add(standing);
            }

            return standings;
        }

        public async Task<JSONArray> GetLeaderboardArrayFromApi(GameType gameType)
        {
            string apiString;
            if (gameType == GameType.Overall) apiString = $"https://leaderboard.viro.fit/api/ChallengeLeaderboards/GetTopPlayersChallengeLeaderboard?ChallengeGuid=2a3fc0f2-4bbb-4902-8137-00775d0b668b";
            else apiString = $"https://leaderboard.viro.fit/api/ChallengeLeaderboards/GetTopPlayersChallengeLeaderboardForLevel?ChallengeGuid=2a3fc0f2-4bbb-4902-8137-00775d0b668b&LevelType={gameType}";

            var resp = await _httpClient.GetAsync(apiString);
            var stringResp = await resp.Content.ReadAsStringAsync();

            JSONNode node = JSON.Parse(WebUtility.UrlDecode(stringResp));
            return node["leaderboard"].AsArray;
        }
    }
}
