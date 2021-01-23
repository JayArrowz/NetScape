using NetScape.Abstractions.Model.Area;
using NetScape.Abstractions.Model.Game;

namespace NetScape.Abstractions.Interfaces.Area
{
    /// <summary>
    ///  A class that should be implemented by listeners that execute actions when an entity is added, moved, or removed from
    ///  a region.
    /// <author>Major</author>
    /// </summary>
    public interface IRegionListener
    {
        void Execute(Region region, Entity entity, EntityUpdateType type);
    }
}
