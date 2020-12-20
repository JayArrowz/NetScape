using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using NetScape.Abstractions.Model.Game;

#nullable disable

namespace NetScape.DAL
{
    public partial class DatabaseContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;

        public DatabaseContext(DbContextOptions<DatabaseContext> options, ILoggerFactory loggerFactory = null)
            : base(options)
        {
            _loggerFactory = loggerFactory;
        }

        public DbSet<Player> Players { get; internal set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "English_United Kingdom.1252");
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
