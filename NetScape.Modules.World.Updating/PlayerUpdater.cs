using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Interfaces.World.Updating;
using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.Area;
using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.World.Updating;
using NetScape.Modules.Messages.Encoders;
using NetScape.Modules.World.Updating.Segments;
using System.Collections.Generic;
using System.Linq;

namespace NetScape.Modules.World.Updating
{
    public class PlayerUpdater : IEntityUpdater<Player>
    {
        private readonly RegionRepository _regionRepository;

        public PlayerUpdater(RegionRepository regionRepository)
        {
            _regionRepository = regionRepository;
        }

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
                entity.SendAsync(new ClearRegionMessage { LocalX = (byte)position.LocalX, LocalY = (byte)position.LocalY });
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


        public void PreUpdate(Player player)
        {
            Position old = player.Position;
            //player.getWalkingQueue().pulse();
            var local = true;

            if (player.IsTeleporting)
            {
                player.ResetViewingDistance();
                local = false;
            }

            Position position = player.Position;
            if (!player.HasLastKnownRegion() || IsRegionUpdateRequired(player))
            {
                player.RegionChanged = true;
                local = false;

                player.LastKnownRegion = position;
                player.SendAsync(new RegionChangeMessage { CentralRegionX = (short) position.CentralRegionX, CentralRegionY = (short) position.CentralRegionY });
            }

            var oldViewable = _regionRepository.FromPosition(old).GetSurrounding();
            var newViewable = _regionRepository.FromPosition(position).GetSurrounding();

            var differences = newViewable.ToHashSet();
            differences.RemoveWhere(t => !oldViewable.Contains(t));

            var full = newViewable.ToHashSet();
            if (local)
            {
                full.RemoveWhere(t => oldViewable.Contains(t));
            }

            SendUpdates(player, player.LastKnownRegion, differences, full);
        }

        /**
         * Sends the updates for a {@link Region}
         *
         * @param position The {@link Position} of the last known region.
         * @param differences The {@link Set} of {@link RegionCoordinates} of Regions that changed.
         * @param full The {@link Set} of {@link RegionCoordinates} of Regions that require a full update.
         */
        private void SendUpdates(Player player, Position position, HashSet<RegionCoordinates> differences, HashSet<RegionCoordinates> full)
        {
            RegionRepository repository = _regionRepository;
            int height = position.Height;

            foreach (RegionCoordinates coordinates in differences)
            {
                var messages = updates.computeIfAbsent(coordinates,
                    coords->repository.get(coords).getUpdates(height));

                if (!messages.isEmpty())
                {
                    player.send(new GroupedRegionUpdateMessage(position, coordinates, messages));
                }
            }

            for (RegionCoordinates coordinates : full)
            {
                Set<RegionUpdateMessage> messages = encodes.computeIfAbsent(coordinates,
                    coords->repository.get(coords).encode(height));

                if (!messages.isEmpty())
                {
                    player.send(new ClearRegionMessage(position, coordinates));
                    player.send(new GroupedRegionUpdateMessage(position, coordinates, messages));
                }
            }
        }


        public void Update(Player entity)
        {
            var lastKnownRegion = entity.LastKnownRegion;
            var regionChanged = entity.RegionChanged;
            var position = entity.Position;
            SynchronizationBlockSet blockSet = entity.BlockSet;
            SynchronizationSegment segment = (entity.IsTeleporting || entity.RegionChanged) ?
                new TeleportSegment(blockSet, position) : new MovementSegment(blockSet, entity.GetDirections());

            if (regionChanged)
            {
                entity.ChannelHandlerContext.WriteAndFlushAsync(new RegionChangeMessage { CentralRegionX = (short)position.CentralRegionX, CentralRegionY = (short)position.CentralRegionY });
            }
        }
    }
}
