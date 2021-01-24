using NetScape.Abstractions.Model.Region;

namespace NetScape.Abstractions.Interfaces.Region
{
    /// <summary>
    /// An entity that can be sent as part of a grouped region update message.
    /// Only <see cref="Model.Game.Entity"/> extensions may implement this interface.
    /// </summary>
    public interface IGroupableEntity
    {
        /// <summary>
        /// Gets this Entity, as an <see cref="IRegionUpdateOperation"/> of a <see cref="IRegion"/>.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="type">The type.</param>
        /// <returns>The UpdateOperation.</returns>
        IRegionUpdateOperation ToUpdateOperation(IRegion region, EntityUpdateType type);
    }
}
