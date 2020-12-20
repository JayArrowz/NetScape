using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using NetScape.Modules.DAL;

namespace NetScape
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            Kernel.SetConfigRoot();
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseNpgsql(Kernel.ConfigurationRoot.GetConnectionString("NetScape"),
                 x => x.MigrationsAssembly(typeof(DatabaseContext)
                    .Assembly.GetName().Name));
            return new DatabaseContext(optionsBuilder.Options);
        }
    }
}
