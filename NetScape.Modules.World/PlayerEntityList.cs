using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using NetScape.Abstractions.Interfaces.World;
using NetScape.Abstractions.Model.Game;

namespace NetScape.Modules.World
{
    public class PlayerEntityList : IEntityList<Player>
    {
        private readonly Player[] _entities = new Player[2048];
        private int _entityCount = 0;
        private List<int> _freeIndexes = new();

        public void Add(Player entity)
        {
            lock (_freeIndexes)
            {
                if (_freeIndexes.Any())
                {
                    var index = _freeIndexes.First();
                    SetIndex(entity, index);
                    return;
                }
            }

            var newIndex = Interlocked.Increment(ref _entityCount);
            SetIndex(entity, newIndex);
        }

        private void SetIndex(Player entity, int index)
        {
            entity.Index = index;
            _entities[index] = entity;
        }

        public void Remove(Player entity)
        {
            _entities[entity.Index] = null;
            _freeIndexes.Remove(entity.Index);
            Interlocked.Decrement(ref _entityCount);
        }

        public Player[] Entities => _entities;

        public int Count => _entityCount;
    }
}
