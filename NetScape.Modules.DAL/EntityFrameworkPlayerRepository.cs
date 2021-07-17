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
    public class EntityFrameworkPlayerRepository<TPlayer> : IPlayerRepository where TPlayer : Player, new()
    {
        private readonly IDbContextFactory<DatabaseContext<TPlayer>> _dbContextFactory;

        public EntityFrameworkPlayerRepository(IDbContextFactory<DatabaseContext<TPlayer>> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<TPlayer> GetAsync(string name)
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {
                var player = await GetAsync(name, dbContext);
                return player;
            }
        }

        async Task<Player> IPlayerRepository.GetOrCreateAsync(PlayerCredentials playerCredentials)
        {
            return await GetOrCreateAsync(playerCredentials);
        }

        public async Task<int> AddOrUpdateAsync(Player player)
        {
            return await AddOrUpdateAsync((TPlayer) player);
        }

        public async Task<int> AddOrUpdateAsync(TPlayer player)
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

        private Task<TPlayer> GetAsync(string name, DatabaseContext<TPlayer> databaseContext)
        {
            var normalizedName = name.ToLower();
            return databaseContext.Players
                .Include(t => t.Appearance)
                .FirstOrDefaultAsync(player => player.Username.ToLower().Equals(normalizedName));
        }

        async Task<Player> IPlayerRepository.GetAsync(string name)
        {
            return await GetAsync(name);
        }

        public async Task<TPlayer> GetOrCreateAsync(PlayerCredentials playerCredentials)
        {
            using (var dbContext = _dbContextFactory.CreateDbContext())
            {

                var playerInDatabase = await GetAsync(playerCredentials.Username, dbContext);
                if (playerInDatabase == null)
                {
                    var defaultPlayer = new TPlayer
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
