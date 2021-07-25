using Autofac;
using NetScape.Abstractions.Interfaces.Region;
using NetScape.Abstractions.Model.Game;

namespace NetScape.Abstractions.Interfaces.World
{
    public interface IWorld : IStartable
    {
        void Add(Player player);
        void Remove(Player player);
        IRegionRepository RegionRepository { get; }
    }
}