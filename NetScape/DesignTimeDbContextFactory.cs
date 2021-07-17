using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using NetScape.Core;
using NetScape.Modules.DAL;

namespace NetScape
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var configRoot = ServerHandler.CreateConfigurationRoot("appsettings.json");
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseNpgsql(configRoot.GetConnectionString("NetScape"),
                 x => x.MigrationsAssembly(typeof(DatabaseContext)
                    .Assembly.GetName().Name));
            return new DatabaseContext(optionsBuilder.Options);
        }
    }
}
