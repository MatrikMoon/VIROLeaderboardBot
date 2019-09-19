using VIROLeaderboardBot.Discord.Database;
using VIROLeaderboardBot;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace VIROLeaderboardBot.Discord.Services
{
    public class DatabaseService
    {
        public DatabaseContext DatabaseContext { get; private set; }

        public DatabaseService(string location, IServiceProvider serviceProvider)
        {
            DatabaseContext = new DatabaseContext(location);
        }

        /*public void RegisterReactionRolesWithBot()
        {
            Logger.Info("Registering saved ReactionRoles...");

            foreach (var reactionRole in DatabaseContext.ReactionRoles)
            {
                if (!reactionRole.Old)
                {
                    Logger.Info($"Registering ReactionRole for {reactionRole.ID} {reactionRole.MessageId} {reactionRole.RoleId} {reactionRole.EmojiId}");

                    _messageUpdateService.ReactionAdded += reactionRole.RoleAdded;
                    _messageUpdateService.ReactionRemoved += reactionRole.RoleRemoved;
                    _messageUpdateService.MessageDeleted += reactionRole.MessageDeleted;
                }
            }
        }*/
    }
}
