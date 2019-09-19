using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;

namespace VIROLeaderboardBot.Discord.Database
{
    [Table(Name = "Scores")]
    public class Score
    {
        [Column(IsPrimaryKey = true, Name = "ID", DbType = "BIGINT", IsDbGenerated = true)]
        [Key]
        public long ID { get; set; }

        [Column(Name = "Ranking")]
        public int Ranking { get; set; }

        [Column(Name = "PlayerName")]
        public string PlayerName { get; set; }

        [Column(Name = "PlayerId")]
        public string PlayerId{ get; set; }

        [Column(Name = "Points")]
        public int Points { get; set; }

        [Column(Name = "GameType")]
        public int GameType { get; set; }

        [Column(Name = "Timestamp")]
        public long Timestamp { get; set; }

        [Column(Name = "Old")]
        public bool Old { get; set; }
    }
}
