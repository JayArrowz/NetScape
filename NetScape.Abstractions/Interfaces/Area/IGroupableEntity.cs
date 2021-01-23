using NetScape.Abstractions.Model.Area;

namespace NetScape.Abstractions.Interfaces.Area
{
    /**
	 * An entity that can be sent as part of a grouped region update message.
	 * <p>
	 * Only {@link org.apollo.game.model.entity.Entity} extensions may implement this interface.
	 * 
	 * @author Major
	 */
    public interface IGroupableEntity
    {

        /**
		 * Gets this Entity, as an {@link UpdateOperation} of a {@link Region}.
		 *
		 * @param region The Region.
		 * @param type The EntityUpdateType.
		 * @return The UpdateOperation.
		 */
        UpdateOperation ToUpdateOperation(Region region, EntityUpdateType type);
    }
}
