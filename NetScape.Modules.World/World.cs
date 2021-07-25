using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Interfaces.World;
using NetScape.Abstractions.Interfaces.World.Updating;
using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.Region;
using NetScape.Modules.Region;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NetScape.Abstractions.Interfaces.Region;

namespace NetScape.Modules.World
{
    public class World : IWorld, IDisposable
    {
        private readonly IEntityUpdater<Player> _playerUpdater;
        private readonly IEntityList<Player> _playerEntityList;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CancellationToken _cancellationToken;
        public IRegionRepository RegionRepository { get; }

        public World(IEntityUpdater<Player> playerUpdater, IRegionRepository regionRepository, IEntityList<Player> playerEntityList)
        {
            _playerUpdater = playerUpdater;
            RegionRepository = regionRepository;
            _playerEntityList = playerEntityList;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
        }

        public void Add(Player player)
        {
            var region = RegionRepository.FromPosition(player.Position);
            _playerEntityList.Add(player);
            player.World = this;
            region.AddEntity(player);
        }

        public void Remove(Player player)
        {
            var region = RegionRepository.FromPosition(player.Position);
            region.RemoveEntity(player);
            _playerEntityList.Remove(player);
        }


        private async Task Process()
        {
            var stopwatch = new Stopwatch();
            while (!_cancellationToken.IsCancellationRequested)
            {
                stopwatch.Restart();
                Dictionary<RegionCoordinates, HashSet<RegionUpdateMessage>> encodes = new(), updates = new();
                for (int playerId = _playerEntityList.Entities.Length - 1; playerId >= 0; playerId--)
                {
                    Player player = _playerEntityList.Entities[playerId];
                    if (player == null)
                    {
                        continue;
                    }
                    try
                    {
                        await _playerUpdater.PreUpdateAsync(player, encodes, updates).ConfigureAwait(false);
                        await _playerUpdater.UpdateAsync(player).ConfigureAwait(false);
                        await _playerUpdater.PostUpdateAsync(player).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error(e, nameof(Process));
                    }
                }

                var deltaSleep = 600 - (int)stopwatch.Elapsed.TotalMilliseconds;
                if (deltaSleep > 0)
                {
                    await Task.Delay(deltaSleep, _cancellationToken).ConfigureAwait(false);
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
