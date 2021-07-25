using System.Collections.Generic;
using NetScape.Abstractions.Model.Game;

namespace NetScape.Abstractions.Interfaces.World
{
    public interface IEntityList<TEntity> where TEntity : Entity
    {
        void Add(TEntity entity);

        void Remove(TEntity entity);

        TEntity[] Entities { get; }

        int Count { get; }
    }
}
