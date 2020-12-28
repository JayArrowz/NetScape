using Autofac;
using NetScape.Abstractions.Interfaces.World.Updating;
using NetScape.Abstractions.Model.Game;
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

        private void Process()
        {
            var stopwatch = new Stopwatch();
            while (!_exited)
            {
                stopwatch.Restart();
                Players.ForEach((player) => {
                    _playerUpdater.PreUpdate(player);
                    _playerUpdater.Update(player);
                    _playerUpdater.PostUpdate(player);
                });
                var deltaSleep = 600 - (int)stopwatch.Elapsed.TotalMilliseconds;
                Thread.Sleep(deltaSleep);
            }
        }

        public void Start()
        {
            var thread = new Thread(new ThreadStart(Process));
            thread.Start();
        }

        public void Dispose()
        {
            _exited = true;
        }
    }
}
