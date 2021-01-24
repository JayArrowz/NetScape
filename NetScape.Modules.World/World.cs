using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Interfaces.World.Updating;
using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.Region;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace NetScape.Modules.World
{
    public class World : IWorld, IDisposable
    {
        private bool _exited = false;
        private readonly IEntityUpdater<Player> _playerUpdater;

        public List<Player> Players { get; } = new List<Player>();

        public World(IEntityUpdater<Player> playerUpdater)
        {
            _playerUpdater = playerUpdater;
        }

        public void Add(Player player)
        {
            Players.Add(player);
        }

        public void Remove(Player player)
        {
            Players.Remove(player);
        }

        private async Task Process()
        {
            var stopwatch = new Stopwatch();
            while (!_exited)
            {
                stopwatch.Restart();
                Dictionary<RegionCoordinates, HashSet<RegionUpdateMessage>> encodes = new(), updates = new();

                foreach (var player in Players)
                {
                    try
                    {
                        await _playerUpdater.PreUpdateAsync(player, encodes, updates);
                        await _playerUpdater.UpdateAsync(player);
                        await _playerUpdater.PostUpdateAsync(player);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error(e, nameof(Process));
                    }
                }
                var deltaSleep = 600 - (int)stopwatch.Elapsed.TotalMilliseconds;
                await Task.Delay(deltaSleep);
            }
        }

        public void Start()
        {
            Task.Factory.StartNew(Process, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void Dispose()
        {
            _exited = true;
        }
    }
}
