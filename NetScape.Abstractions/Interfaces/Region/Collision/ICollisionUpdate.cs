using Microsoft.Collections.Extensions;
using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.Region.Collision;

namespace NetScape.Abstractions.Interfaces.Region.Collision
{
    /// <summary>
    /// A global update to the collision matrices.
    /// </summary>
    public interface ICollisionUpdate
    {
        /// <summary>
        /// Gets or sets the flags.
        /// </summary>
        /// <value>
        /// A mapping of <see cref="Position"/>s to their <see cref="DirectionFlag"/>s.
        /// </value>
        MultiValueDictionary<Position, DirectionFlag> Flags { get; set; }

        /// <summary>
        /// Gets or sets the type of update.
        /// </summary>
        /// <value>
        /// The type of this update.
        /// </value>
        CollisionUpdateType Type { get; set; }
    }
}