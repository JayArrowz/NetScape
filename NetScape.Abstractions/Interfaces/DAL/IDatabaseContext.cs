using Microsoft.EntityFrameworkCore;
using NetScape.Abstractions.Model.Game;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetScape.Abstractions.Interfaces.DAL
{
    public interface IDatabaseContext : IDisposable
    {
        DbSet<Player> Players { get; set; }
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
