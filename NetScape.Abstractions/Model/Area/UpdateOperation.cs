using Dawn;
using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Model.Game;
using System;

namespace NetScape.Abstractions.Model.Area
{
    public abstract class UpdateOperation
    {
        /**
	     * The Entity involved in this UpdateOperation.
	     */
        protected readonly Entity entity;

        /**
         * The Region in which this type occurred.
         */
        protected readonly Region region;

        /**
         * The type of update.
         */
        protected readonly EntityUpdateType type;

        /**
         * Creates the UpdateOperation.
         *
         * @param region The region in which the UpdateOperation occurred. Must not be {@code null}.
         * @param type The type of {@link EntityUpdateType}. Must not be {@code null}.
         * @param entity The {@link Entity} being added or removed. Must not be {@code null}.
         */
        public UpdateOperation(Region region, EntityUpdateType type, Entity entity)
        {
            this.region = region;
            this.type = type;
            this.entity = entity;
        }

        /**
         * Gets a {@link RegionUpdateMessage} that would counteract the effect of this UpdateOperation.
         *
         * @return The RegionUpdateMessage.
         */
        public RegionUpdateMessage Inverse()
        {
            int offset = GetPositionOffset(entity.Position);

            switch (type)
            {
                case EntityUpdateType.Add:
                    return Remove(offset);
                case EntityUpdateType.Remove:
                    return Add(offset);
                default:
                    throw new ArgumentException("Unsupported EntityUpdateType " + type + ".");
            }
        }

        /**
		 * Returns this UpdateOperation as a {@link Message}.
		 *
		 * @return The Message.
		 */
        public RegionUpdateMessage ToMessage()
        {
            int offset = GetPositionOffset(entity.Position);

            switch (type)
            {
                case EntityUpdateType.Add:
                    return Add(offset);
                case EntityUpdateType.Remove:
                    return Remove(offset);
                default:
                    throw new ArgumentException("Unsupported EntityUpdateType " + type + ".");
            }
        }

        /**
		 * Returns a {@link RegionUpdateMessage} that adds the {@link Entity} in this UpdateOperation.
		 *
		 * @param offset The offset of the {@link Position} of the Entity from the Position of the {@link Region}.
		 * @return The RegionUpdateMessage.
		 */
        protected abstract RegionUpdateMessage Add(int offset);

        /**
		 * Returns a {@link RegionUpdateMessage} that removes the {@link Entity} in this UpdateOperation.
		 *
		 * @param offset The offset of the {@link Position} of the Entity from the Position of the {@link Region}.
		 * @return The RegionUpdateMessage.
		 */
        protected abstract RegionUpdateMessage Remove(int offset);

        /**
		 * Gets the position offset for the specified {@link Position}.
		 *
		 * @param position The Position.
		 * @return The position offset.
		 */
        private int GetPositionOffset(Position position)
        {
            var coordinates = region.Coordinates;
            int dx = position.X - coordinates.AbsoluteX;
            int dy = position.Y - coordinates.AbsoluteY;

            Guard.Argument(dx).InRange(0, Region.Size);
            Guard.Argument(dy).InRange(0, Region.Size);
            return dx << 4 | dy;
        }
    }
}
