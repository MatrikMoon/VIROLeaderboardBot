using System;
using System.Collections.Generic;
using System.Timers;
using VIROLeaderboardBot.Discord.Database;

namespace VIROLeaderboardBot.Discord.Services
{
    class LeaderboardService
    {
        public DatabaseService DatabaseService { get; set; }

        public event Action<List<Score>> OverallLeaderboardsChanged;
        public event Action<List<Score>> BoxingLeaderboardsChanged;
        public event Action<List<Score>> ShootingLeaderboardsChanged;
        public event Action<List<Score>> SwordplayLeaderboardsChanged;
        public event Action<List<Score>> WeaponMasterLeaderboardsChanged;

        private Timer _pollTimer = new Timer();

        public void StartPolling(long interval)
        {
            _pollTimer.Interval = interval;
            _pollTimer.Elapsed += pollTimer_Elapsed;
            _pollTimer.Start();
        }

        private void pollTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Poll();
        }


        public void Poll()
        {

        }
    }
}
