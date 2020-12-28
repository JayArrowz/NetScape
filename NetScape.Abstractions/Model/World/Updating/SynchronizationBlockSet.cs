using System;
using System.Collections.Generic;

namespace NetScape.Abstractions.Model.World.Updating
{
    public class SynchronizationBlockSet : ICloneable
    {
        private readonly Dictionary<Type, SynchronizationBlock> _blocks = new Dictionary<Type, SynchronizationBlock>(8);

        public object Clone()
        {
            SynchronizationBlockSet copy = new SynchronizationBlockSet();
            foreach (var block in _blocks)
            {
                copy._blocks.Add(block.Key, block.Value);
            }
            return copy;
        }

        public void Add<T>(T block) where T : SynchronizationBlock
        {
            _blocks.Add(typeof(T), block);
        }

        public bool Contains<T>() where T : SynchronizationBlock
        {
            return _blocks.ContainsKey(typeof(T));
        }

        public T Get<T>() where T : SynchronizationBlock
        {
            var containsBlock = _blocks.TryGetValue(typeof(T), out var block);
            return containsBlock ? (T)block : null;
        }

        public T Remove<T>() where T : SynchronizationBlock
        {
            var block = Get<T>();
            if (block != null)
            {
                _blocks.Remove(typeof(T));
            }
            return block;
        }

        public int Size()
        {
            return _blocks.Count;
        }

        public void Clear()
        {
            _blocks.Clear();
        }
    }
}
