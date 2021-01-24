using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.Region;

namespace NetScape.Abstractions.Interfaces.Region
{
    /// <summary>
    ///  A class that should be implemented by listeners that execute actions when an entity is added, moved, or removed from
    ///  a region.
    /// <author>Major</author>
    /// </summary>
    public interface IRegionListener
    {
        void Execute(IRegion region, Entity entity, EntityUpdateType type);
    }
}
