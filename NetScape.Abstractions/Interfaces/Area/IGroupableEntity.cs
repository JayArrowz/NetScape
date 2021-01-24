using NetScape.Abstractions.Model.Area;

namespace NetScape.Abstractions.Interfaces.Area
{
	/// <summary>
	/// An entity that can be sent as part of a grouped region update message.
	/// Only <see cref="Model.Game.Entity"/> extensions may implement this interface.
	/// </summary>
	public interface IGroupableEntity
    {
		/// <summary>
		/// Gets this Entity, as an <see cref="UpdateOperation"/> of a <see cref="Region"/>.
		/// </summary>
		/// <param name="region">The region.</param>
		/// <param name="type">The type.</param>
		/// <returns>The UpdateOperation.</returns>
		UpdateOperation ToUpdateOperation(Region region, EntityUpdateType type);
    }
}
