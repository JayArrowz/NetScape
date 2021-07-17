using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using NetScape.Abstractions.Model.Game;
using NetScape.Core;
using NetScape.Modules.DAL;

namespace NetScape
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DatabaseContext<Player>>
    {
        public DatabaseContext<Player> CreateDbContext(string[] args)
        {
            var configRoot = ServerHandler.CreateConfigurationRoot("appsettings.json");
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext<Player>>();
            optionsBuilder.UseNpgsql(configRoot.GetConnectionString("NetScape"),
                 x => x.MigrationsAssembly(typeof(DatabaseContext<Player>)
                    .Assembly.GetName().Name));
            return new DatabaseContext<Player>(optionsBuilder.Options);
        }
    }
}
