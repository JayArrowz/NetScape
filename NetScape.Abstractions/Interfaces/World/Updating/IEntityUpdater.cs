using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.Region;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetScape.Abstractions.Interfaces.World.Updating
{
    public interface IEntityUpdater<T> where T : Entity
    {
        Task PreUpdateAsync(T entity, Dictionary<RegionCoordinates, HashSet<RegionUpdateMessage>> encodes,
            Dictionary<RegionCoordinates, HashSet<RegionUpdateMessage>> updates);

        Task UpdateAsync(T entity);
        Task PostUpdateAsync(T entity);
    }
}
