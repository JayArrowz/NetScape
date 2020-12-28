using NetScape.Abstractions.Interfaces.World.Updating;
using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.Area;
using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.World.Updating;
using NetScape.Modules.Messages.Encoders;
using NetScape.Modules.World.Updating.Segments;
using System.Collections.Generic;

namespace NetScape.Modules.World.Updating
{
    public class PlayerUpdater : IEntityUpdater<Player>
    {
        public void PostUpdate(Player entity)
        {
            Position old = entity.Position;
            var local = true;

            if (entity.IsTeleporting)
            {
                entity.ResetViewingDistance();
                local = false;
            }

            Position position = entity.Position;

            if (!entity.HasLastKnownRegion() || IsRegionUpdateRequired(entity))
            {
                entity.RegionChanged = true;
                local = false;

                entity.LastKnownRegion = position;
                entity.ChannelHandlerContext.WriteAndFlushAsync(new RegionChangeMessage { CentralRegionX = (short)position.CentralRegionX, CentralRegionY = (short)position.CentralRegionY });
            }
        }

        private bool IsRegionUpdateRequired(Player player)
        {
            Position current = player.Position;
            Position last = player.LastKnownRegion;

            int deltaX = current.GetLocalX(last);
            int deltaY = current.GetLocalY(last);

            return deltaX <= Position.MaxDistance || deltaX >= Region.Viewport_Width - Position.MaxDistance - 1
                || deltaY <= Position.MaxDistance || deltaY >= Region.Viewport_Width - Position.MaxDistance - 1;
        }


        public void PreUpdate(Player entity)
        {
        }

        public void Update(Player entity)
        {
            var lastKnownRegion = entity.LastKnownRegion;
            var regionChanged = entity.RegionChanged;
            var position = entity.Position;
            SynchronizationBlockSet blockSet = entity.BlockSet;
            SynchronizationSegment segment = (entity.IsTeleporting || entity.RegionChanged) ?
                new TeleportSegment(blockSet, position) : new MovementSegment(blockSet, entity.GetDirections());

        }
    }
}
