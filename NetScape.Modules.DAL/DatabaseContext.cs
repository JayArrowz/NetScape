using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using NetScape.Abstractions.Model.Game;

#nullable disable

namespace NetScape.Modules.DAL
{
    public partial class DatabaseContext<TPlayer> : DbContext where TPlayer : Player
    {
        private readonly ILoggerFactory _loggerFactory;

        public DatabaseContext(DbContextOptions<DatabaseContext<TPlayer>> options, ILoggerFactory loggerFactory = null)
            : base(options)
        {
            _loggerFactory = loggerFactory;
        }

        public DbSet<TPlayer> Players { get; set; }
        public DbSet<Appearance> Appearances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "English_United Kingdom.1252");
            modelBuilder
                .Entity<Appearance>()
                .Property(e => e.Colors)
                .HasConversion(v => string.Join(",", v), v => v.Split(",", StringSplitOptions.None).Select(int.Parse).ToArray());
            modelBuilder
                .Entity<Appearance>()
                .Property(e => e.Style)
                .HasConversion(v => string.Join(",", v), v => v.Split(",", StringSplitOptions.None).Select(int.Parse).ToArray());
            modelBuilder.Entity<Player>().OwnsOne(t => t.Position);
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
