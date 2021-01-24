using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.Region;
using System.Collections.Generic;

namespace NetScape.Abstractions.Interfaces.Region
{
    public interface IRegionRepository
    {
        void AddRegionListener(IRegionListener listener);
        bool Contains(IRegion region);
        bool Contains(RegionCoordinates coordinates);
        IRegion FromPosition(Position position);
        IRegion Get(RegionCoordinates coordinates);
        List<IRegion> GetRegions();
        bool Remove(IRegion region);
    }
}