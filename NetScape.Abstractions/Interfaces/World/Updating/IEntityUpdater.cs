using NetScape.Abstractions.Model.Game;

namespace NetScape.Abstractions.Interfaces.World.Updating
{
    public interface IEntityUpdater<T> where T : Entity
    {
        void PreUpdate(T entity);
        void Update(T entity);
        void PostUpdate(T entity);
    }
}
