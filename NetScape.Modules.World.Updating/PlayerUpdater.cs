using NetScape.Abstractions.Interfaces.Messages;
using NetScape.Abstractions.Interfaces.Region;
using NetScape.Abstractions.Interfaces.World.Updating;
using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.Game;
using NetScape.Abstractions.Model.Region;
using NetScape.Abstractions.Model.World.Updating;
using NetScape.Abstractions.Model.World.Updating.Blocks;
using NetScape.Modules.Messages.Encoders;
using NetScape.Modules.Messages.Outgoing;
using NetScape.Modules.Messages.Region;
using NetScape.Modules.World.Updating.Segments;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetScape.Modules.World.Updating
{
    public class PlayerUpdater : IEntityUpdater<Player>
    {
        private readonly IRegionRepository _regionRepository;

        private static readonly int MaximumLocalPlayers = 255;

        /// <summary>
        /// The maximum number of players to load per cycle. This prevents the update packet from becoming too large (the
        /// client uses a 5000 byte buffer) and also stops old spec PCs from crashing when they login or teleport.
        /// </summary>
        private static readonly int NewPlayersPerCycle = 20;

        public PlayerUpdater(IRegionRepository regionRepository)
        {
            _regionRepository = regionRepository;
        }

        public Task PostUpdateAsync(Player player)
        {
            player.IsTeleporting = false;
            player.RegionChanged = false;
            player.BlockSet = new SynchronizationBlockSet();

            if (!player.ExcessivePlayers)
            {
                player.IncrementViewingDistance();
            }
            else
            {
                player.DecrementViewingDistance();
                player.ExcessivePlayers = false;
            }
            return Task.CompletedTask;
        }

        private bool IsRegionUpdateRequired(Player player)
        {
            Position current = player.Position;
            Position last = player.LastKnownRegion;

            int deltaX = current.GetLocalX(last);
            int deltaY = current.GetLocalY(last);

            return deltaX <= Position.MaxDistance || deltaX >= IRegion.Viewport_Width - Position.MaxDistance - 1
                || deltaY <= Position.MaxDistance || deltaY >= IRegion.Viewport_Width - Position.MaxDistance - 1;
        }


        public async Task PreUpdateAsync(Player player, Dictionary<RegionCoordinates, HashSet<RegionUpdateMessage>> encodes,
            Dictionary<RegionCoordinates, HashSet<RegionUpdateMessage>> updates)
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
                await player.SendAsync(new RegionChangeMessage { CentralRegionX = (short)position.CentralRegionX, CentralRegionY = (short)position.CentralRegionY });
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

            await SendUpdates(player, player.LastKnownRegion, differences, full, encodes, updates);
        }

        /// <summary>
        /// Sends the updates for a <see cref="Region"/>
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="position">The position.</param>
        /// <param name="differences">The differences.</param>
        /// <param name="full">The HashSet of <see cref="RegionCoordinates"/> of regions that require a full update.</param>
        /// <param name="encodes">The HashSet of <see cref="RegionCoordinates"/> of regions that changed.</param>
        /// <param name="updates">The HashSet of <see cref="RegionCoordinates"/> of regions that require a update.</param>
        private async Task SendUpdates(Player player, Position position, HashSet<RegionCoordinates> differences, HashSet<RegionCoordinates> full, Dictionary<RegionCoordinates, HashSet<RegionUpdateMessage>> encodes, Dictionary<RegionCoordinates, HashSet<RegionUpdateMessage>> updates)
        {
            IRegionRepository repository = _regionRepository;
            int height = position.Height;

            foreach (RegionCoordinates coordinates in differences)
            {
                var updatesMsgs = repository.Get(coordinates).GetUpdates(height);
                var messages = updates.TryAdd(coordinates, updatesMsgs);

                if (messages)
                {
                    await player.SendAsync(new GroupedRegionUpdateMessage(position, coordinates, updatesMsgs));
                }
            }

            foreach (RegionCoordinates coordinates in full)
            {
                var addMessages = repository.Get(coordinates).Encode(height);
                var added = encodes.TryAdd(coordinates, addMessages);

                if (added)
                {
                    await player.SendAsync(new ClearRegionMessage { LocalX = (byte)position.LocalX, LocalY = (byte)position.LocalX });
                    await player.SendAsync(new GroupedRegionUpdateMessage(position, coordinates, addMessages));
                }
            }
        }

        /// <summary>
        /// Determines whether [player has cached appearance] [the specified appearance tickets].
        /// </summary>
        /// <param name="appearanceTickets">The appearance tickets.</param>
        /// <param name="index">The index of the player.</param>
        /// <param name="appearanceTicket">The appearance ticket.</param>
        /// <returns>
        ///   <c>true</c> if [player has cached appearance] [the specified appearance tickets]; otherwise, <c>false</c>.
        /// </returns>
        private bool HasCachedAppearance(int[] appearanceTickets, int index, int appearanceTicket)
        {
            if (appearanceTickets[index] != appearanceTicket)
            {
                appearanceTickets[index] = appearanceTicket;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns whether or not the specified <see cref="Player"/> should be removed.
        /// </summary>
        /// <param name="position">The position of the Player being updated.</param>
        /// <param name="distance">The distance.</param>
        /// <param name="other">The Player being tested.</param>
        /// <returns></returns>
        private bool Removeable(Position position, int distance, Player other)
        {
            if (other.IsTeleporting || !other.IsActive)
            {
                return true;
            }

            Position otherPosition = other.Position;
            return otherPosition.GetLongestDelta(position) > distance || !otherPosition.IsWithinDistance(position, distance);
        }

        public async Task UpdateAsync(Player player)
        {
            Position lastKnownRegion = player.LastKnownRegion;
            var regionChanged = player.RegionChanged;
            int[] appearanceTickets = player.AppearanceTickets;

            SynchronizationBlockSet blockSet = player.BlockSet;

            Position position = player.Position;

            SynchronizationSegment segment = (player.IsTeleporting || player.RegionChanged) ?
                    new TeleportSegment(blockSet, position) : new MovementSegment(blockSet, player.GetDirections());

            List<Player> localPlayers = player.LocalPlayerList;
            int oldCount = localPlayers.Count;

            List<SynchronizationSegment> segments = new();
            int distance = player.ViewingDistance;

            foreach (var other in localPlayers.ToList())
            {
                if (Removeable(position, distance, other))
                {
                    localPlayers.Remove(other);
                    segments.Add(new RemoveMobSegment());
                }
                else
                {
                    segments.Add(new MovementSegment(other.BlockSet, other.GetDirections()));
                }
            }

            int added = 0, count = localPlayers.Count();

            IRegion current = _regionRepository.FromPosition(position);
            HashSet<RegionCoordinates> regions = current.GetSurrounding();
            regions.Add(current.Coordinates);

            IEnumerable<Player> players = regions.Select(t => _regionRepository.Get(t))
                    .SelectMany(region => region.GetEntities<Player>(EntityType.Player));

            foreach (var other in players)
            {
                if (count >= MaximumLocalPlayers)
                {
                    player.ExcessivePlayers = true;
                    break;
                }
                else if (added >= NewPlayersPerCycle)
                {
                    break;
                }

                Position local = other.Position;

                if (other != player && local.IsWithinDistance(position, distance) && !localPlayers.Contains(other))
                {
                    localPlayers.Add(other);
                    count++;
                    added++;

                    blockSet = other.BlockSet;

                    int index = other.Index;

                    if (!blockSet.Contains<AppearanceBlock>() && !HasCachedAppearance(appearanceTickets, index - 1, other.AppearanceTicket))
                    {
                        blockSet = (SynchronizationBlockSet)blockSet.Clone();
                        blockSet.Add(SynchronizationBlock.CreateAppearanceBlock(other));
                    }

                    segments.Add(new AddPlayerSegment(blockSet, index, local));
                }
            }

            PlayerSynchronizationMessage message = new PlayerSynchronizationMessage(lastKnownRegion, position,
                    regionChanged, segment, oldCount, segments);
            await player.SendAsync(message);
        }
    }
}
