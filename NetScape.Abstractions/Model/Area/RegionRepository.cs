using NetScape.Abstractions.Interfaces.Area;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetScape.Abstractions.Model.Area
{
    /**
	 * A repository of {@link Region}s, backed by a {@link HashMap} of {@link RegionCoordinates} that correspond to their
	 * appropriate regions.
	 *
	 * @author Major
	 */
    public class RegionRepository
    {

        /**
		 * Returns an immutable RegionRepository, where {@link Region}s cannot be added or removed.
		 * <p>
		 * Note that, internally, regions are added lazily (i.e. only when necessary). As such, repositories are (again,
		 * internally) not actually immutable, so do not rely on such behaviour.
		 *
		 * @return The RegionRepository.
		 */
        public RegionRepository() : this(false) { }

        /**
		 * Returns a mutable RegionRepository, where {@link Region}s may be removed.
		 *
		 * @return The RegionRepository.
		 */
        public static RegionRepository Mutable()
        {
            return new RegionRepository(true);
        }

        /**
		 * Whether or not regions can be removed from this repository.
		 */
        private readonly bool permitRemoval;

        /**
         * The map of RegionCoordinates that correspond to the appropriate Regions.
         */
        private readonly Dictionary<RegionCoordinates, Region> _regions = new();

        /**
		 * A list of default {@link RegionListener}s which will be added to {@link Region}s upon creation.
		 */
        private readonly List<IRegionListener> _defaultRegionListeners = new();

        /**
		 * Creates a new RegionRepository.
		 *
		 * @param permitRemoval If removal (of {@link Region}s) from this repository should be permitted.
		 */
        private RegionRepository(bool permitRemoval)
        {
            this.permitRemoval = permitRemoval;
        }

        /**
		 * Adds a {@link Region} to the repository.
		 *
		 * @param region The Region.
		 * @throws IllegalArgumentException If the provided Region is null.
		 * @throws UnsupportedOperationException If the coordinates of the provided Region are already mapped (and hence the
		 *             existing Region would be replaced), and removal of regions is not permitted.
		 */
        private void Add(Region region)
        {
            if (_regions.ContainsKey(region.Coordinates) && !permitRemoval)
            {
                throw new InvalidOperationException("Cannot add a Region with the same coordinates as an existing Region.");
            }

            _defaultRegionListeners.ForEach(region.AddListener);
            _regions.Add(region.Coordinates, region);
        }

        /**
		 * Adds a {@link RegionListener} to be registered as a default listener with all newly created {@link Region}s and
		 * associated with any existing instances.
		 *
		 * @param listener The listener to add.
		 */
        public void AddRegionListener(IRegionListener listener)
        {
            foreach (Region region in _regions.Values)
            {
                region.AddListener(listener);
            }

            _defaultRegionListeners.Add(listener);
        }

        /**
		 * Indicates whether the supplied value (i.e. the {@link Region}) has a mapping.
		 *
		 * @param region The Region.
		 * @return {@code true} if this repository contains an entry with {@link RegionCoordinates} equal to the specified
		 *         Region, otherwise {@code false}.
		 */
        public bool Contains(Region region)
        {
            return Contains(region.Coordinates);
        }

        /**
		 * Indicates whether the supplied key (i.e. the {@link RegionCoordinates}) has a mapping.
		 *
		 * @param coordinates The coordinates.
		 * @return {@code true} if the key is already mapped to a value (i.e. a {@link Region}), otherwise {@code false}.
		 */
        public bool Contains(RegionCoordinates coordinates)
        {
            return _regions.ContainsKey(coordinates);
        }

        /**
		 * Gets the {@link Region} that contains the specified {@link Position}. If the Region does not exist in this
		 * repository then a new Region is created, submitted to the repository, and returned.
		 *
		 * @param position The position.
		 * @return The Region.
		 */
        public Region FromPosition(Position position)
        {
            return Get(RegionCoordinates.FromPosition(position));
        }

        /**
		 * Gets a {@link Region} with the specified {@link RegionCoordinates}. If the Region does not exist in this
		 * repository then a new Region is created, submitted to the repository, and returned.
		 *
		 * @param coordinates The RegionCoordinates.
		 * @return The Region. Will never be null.
		 */
        public Region Get(RegionCoordinates coordinates)
        {
            var valid = _regions.TryGetValue(coordinates, out var region);
            if (!valid)
            {
                region = new Region(coordinates);
                Add(region);
            }

            return region;
        }

        /**
		 * Gets a shallow copy of the {@link List} of {@link Region}s. This will be an {@link ImmutableList}.
		 *
		 * @return The List.
		 */
        public List<Region> GetRegions()
        {
            return _regions.Values.ToList();
        }

        /**
		 * Removes a {@link Region} from the repository, if permitted. This method removes the entry that has a key
		 * identical to the {@link RegionCoordinates} of the specified Region.
		 *
		 * @param region The Region to remove.
		 * @return {@code true} if the specified Region existed and was removed, {@code false} if not.
		 * @throws UnsupportedOperationException If this method is called on a repository that does not permit removal.
		 */
        public bool Remove(Region region)
        {
            if (!permitRemoval)
            {
                throw new NotSupportedException("Cannot remove regions from this repository.");
            }

            return _regions.Remove(region.Coordinates);
        }

    }
}
