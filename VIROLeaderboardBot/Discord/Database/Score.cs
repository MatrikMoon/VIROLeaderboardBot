using Discord;
using Discord.WebSocket;
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

        [Column(Name = "PlayerId")]
        public long PlayerId { get; set; }

        [Column(Name = "Old")]
        public bool Old { get; set; }
    }
}
