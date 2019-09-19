using SQLite.CodeFirst;
using System.Data.Entity;
using System.Data.SQLite;

namespace VIROLeaderboardBot.Discord.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(string location) :
            base(new SQLiteConnection()
            {
                ConnectionString = new SQLiteConnectionStringBuilder() { DataSource = location }.ConnectionString
            }, true)
        { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<DatabaseContext>(modelBuilder);
            System.Data.Entity.Database.SetInitializer(sqliteConnectionInitializer);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Score> Scores { get; set; }
    }
}
