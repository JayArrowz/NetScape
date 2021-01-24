using Dawn;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Interfaces.Region;
using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.Region;
using System;

namespace NetScape.Modules.Region
{
    public abstract class RegionUpdateOperation : IRegionUpdateOperation
    {
        /// <summary>
        /// The Entity involved in this UpdateOperation.
        /// </summary>
        protected readonly Entity _entity;

        /// <summary>
        /// The Region in which this type occurred.
        /// </summary>
        protected readonly IRegion _region;

        /// <summary>
        /// The type of update.
        /// </summary>
        protected readonly EntityUpdateType _type;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionUpdateOperation"/> class.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="type">The type.</param>
        /// <param name="entity">The entity.</param>
        public RegionUpdateOperation(IRegion region, EntityUpdateType type, Entity entity)
        {
            Guard.Argument(region).NotNull();
            Guard.Argument(entity).NotNull();
            _region = region;
            _type = type;
            _entity = entity;
        }

        /// <summary>
        /// Gets a <see cref="RegionUpdateMessage"/> that would counteract the effect of this UpdateOperation.
        /// </summary>
        /// <returns>The RegionUpdateMessage</returns>
        /// <exception cref="ArgumentException">Unsupported EntityUpdateType</exception>
        public RegionUpdateMessage Inverse()
        {
            int offset = GetPositionOffset(_entity.Position);

            switch (_type)
            {
                case EntityUpdateType.Add:
                    return Remove(offset);
                case EntityUpdateType.Remove:
                    return Add(offset);
                default:
                    throw new ArgumentException("Unsupported EntityUpdateType " + _type + ".");
            }
        }

        /// <summary>
        /// Returns this UpdateOperation as a RegionUpdateMessage.
        /// </summary>
        /// <returns>RegionUpdateMessage</returns>
        /// <exception cref="ArgumentException">Unsupported EntityUpdateType</exception>
        public RegionUpdateMessage ToMessage()
        {
            int offset = GetPositionOffset(_entity.Position);

            switch (_type)
            {
                case EntityUpdateType.Add:
                    return Add(offset);
                case EntityUpdateType.Remove:
                    return Remove(offset);
                default:
                    throw new ArgumentException("Unsupported EntityUpdateType " + _type + ".");
            }
        }

        /// <summary>
        /// Returns a <see cref="RegionUpdateMessage"/> that adds the Entity in this <see cref="RegionUpdateOperation"/>.
        /// </summary>
        /// <param name="offset">The offset of the <see cref="Position"/> of the Entity from the Position of the <see cref="Region"/>..</param>
        /// <returns>The RegionUpdateMessage</returns>
        protected abstract RegionUpdateMessage Add(int offset);

        /// <summary>
        /// Returns a <see cref="RegionUpdateMessage"/> that removes the <see cref="Entity"/> in this UpdateOperation.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns>The RegionUpdateMessage</returns>
        protected abstract RegionUpdateMessage Remove(int offset);

        /// <summary>
        /// Gets the position offset for the specified <see cref="Position"/>.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        private int GetPositionOffset(Position position)
        {
            var coordinates = _region.Coordinates;
            int dx = position.X - coordinates.AbsoluteX;
            int dy = position.Y - coordinates.AbsoluteY;

            Guard.Argument(dx).InRange(0, IRegion.Size);
            Guard.Argument(dy).InRange(0, IRegion.Size);
            return dx << 4 | dy;
        }
    }
}
