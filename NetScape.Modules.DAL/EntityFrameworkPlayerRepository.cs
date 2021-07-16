using Microsoft.EntityFrameworkCore;
using NetScape.Abstractions;
using NetScape.Abstractions.FileSystem;
using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.Login;
using Serilog;
using System;
using System.Threading.Tasks;

namespace NetScape.Modules.DAL
{
    /// <summary>
    /// Serializes/Deserializes players using entity framework
    /// </summary>
    /// <seealso cref="IPlayerRepository" />
    public class EntityFrameworkPlayerRepository : IPlayerRepository
    {
        private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;

        public EntityFrameworkPlayerRepository(IDbContextFactory<DatabaseContext> dbContextFactory)
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
                try
                {
                    var playerInDatabase = await GetAsync(player.Username, dbContext);

                    if (playerInDatabase == null)
                    {
                        dbContext.Attach(player);
                        dbContext.Add(player);
                    }
                    else
                    {
                        dbContext.Entry(playerInDatabase).CurrentValues.SetValues(player);
                        dbContext.Entry(playerInDatabase).Reference(t => t.Position).CurrentValue = player.Position;
                        dbContext.Update(playerInDatabase);
                    }
                    var rowsModified = await dbContext.SaveChangesAsync();
                    return rowsModified;
                } catch(Exception e)
                {
                    Log.Logger.Error(e, nameof(AddOrUpdateAsync));
                    throw;
                }
            }
        }

        private Task<Player> GetAsync(string name, DatabaseContext databaseContext)
        {
            var normalizedName = name.ToLower();
            return databaseContext.Players
                .Include(t => t.Appearance)
                .FirstOrDefaultAsync(player => player.Username.ToLower().Equals(normalizedName));
        }

        public async Task<Player> GetOrCreateAsync(PlayerCredentials playerCredentials)
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {

                var playerInDatabase = await GetAsync(playerCredentials.Username, dbContext);
                if (playerInDatabase == null)
                {
                    var defaultPlayer = new Player
                    {
                        Username = playerCredentials.Username,
                        Password = playerCredentials.Password,
                        Position = Constants.HomePosition,
                        Appearance = Appearance.DefaultAppearance
                    };
                    dbContext.Attach(defaultPlayer);
                    dbContext.Add(defaultPlayer.Position); 
                    dbContext.Add(defaultPlayer);
                    await dbContext.SaveChangesAsync();
                    return defaultPlayer;
                }
                return playerInDatabase;
            }
        }
    }
}
