using Microsoft.EntityFrameworkCore;
using NetScape.Abstractions.FileSystem;
using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.IO.Login;
using System;
using System.Threading.Tasks;

namespace NetScape.Modules.DAL
{
    public class EntityFrameworkPlayerSerializer : IPlayerSerializer
    {
        private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;

        public EntityFrameworkPlayerSerializer(IDbContextFactory<DatabaseContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<Player> GetAsync(string name)
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                var player = await GetAsync(name, dbContext);
                return player;
            }
        }

        public async Task<int> AddOrUpdateAsync(Player player)
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                var playerInDatabase = await GetAsync(player.Username, dbContext);

                if (playerInDatabase == null)
                {
                    dbContext.Attach(player);
                    dbContext.Add(player);
                }
                else
                {
                    dbContext.Update(player);
                }
                var rowsModified = await dbContext.SaveChangesAsync();
                return rowsModified;
            }
        }

        private Task<Player> GetAsync(string name, DatabaseContext databaseContext)
        {
            var normalizedName = name.ToLower();
            return databaseContext.Players.FirstOrDefaultAsync(player => player.Username.ToLower().Equals(normalizedName));
        }

        public async Task<Player> GetOrCreateAsync(PlayerCredentials playerCredentials)
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {

                var playerInDatabase = await GetAsync(playerCredentials.Username, dbContext);
                if(playerInDatabase == null)
                {
                    var defaultPlayer = new Player { Username = playerCredentials.Username, Password = playerCredentials.Password, Position = new Position(3333, 3333) };
                    dbContext.Attach(defaultPlayer);
                    dbContext.Add(defaultPlayer);
                    await dbContext.SaveChangesAsync();
                    return defaultPlayer;
                }
                return playerInDatabase;
            }
        }
    }
}
