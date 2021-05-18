using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Interfaces.World;
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
        private readonly IEntityUpdater<Player> _playerUpdater;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CancellationToken _cancellationToken;
        public List<Player> Players { get; } = new List<Player>();

        public World(IEntityUpdater<Player> playerUpdater)
        {
            _playerUpdater = playerUpdater;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
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
            while (!_cancellationToken.IsCancellationRequested)
            {
                stopwatch.Restart();
                Dictionary<RegionCoordinates, HashSet<RegionUpdateMessage>> encodes = new(), updates = new();
                for (int playerId = Players.Count - 1; playerId >= 0; playerId--)
                {
                    Player player = Players[playerId];
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
                if (deltaSleep > 0)
                {
                    await Task.Delay(deltaSleep, _cancellationToken);
                }
            }
        }

        public void Start()
        {
            Task.Factory.StartNew(Process, _cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}
