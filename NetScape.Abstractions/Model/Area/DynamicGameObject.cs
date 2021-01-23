using NetScape.Abstractions.Model.Game;
using NetScape.Modules.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetScape.Abstractions.Model.Area
{
    public class DynamicGameObject : GameObject
    {
        
        /**
	     * Creates a DynamicGameObject that is visible only to {@link Player}s specified later.
	     *
	     * @param world The {@link World} containing the DynamicGameObject.
	     * @param id The id of the DynamicGameObject
	     * @param position The {@link Position} of the DynamicGameObject.
	     * @param type The type of the DynamicGameObject.
	     * @param orientation The orientation of the DynamicGameObject.
	     * @return The DynamicGameObject.
	     */
        public static DynamicGameObject CreateLocal(int id, Position position, int type, int orientation)
        {
            return new DynamicGameObject(id, position, type, orientation, false);
        }

        /**
		 * Creates a DynamicGameObject that is always visible.
		 *
		 * @param world The {@link World} containing the DynamicGameObject.
		 * @param id The id of the DynamicGameObject
		 * @param position The {@link Position} of the DynamicGameObject.
		 * @param type The type of the DynamicGameObject.
		 * @param orientation The orientation of the DynamicGameObject.
		 * @return The DynamicGameObject.
		 */
        public static DynamicGameObject CreatePublic(int id, Position position, int type,
                int orientation)
        {
            return new DynamicGameObject(id, position, type, orientation, true);
        }

        /**
		 * The flag indicating whether or not this DynamicGameObject is visible to every player.
		 */
        private readonly bool alwaysVisible;

        /**
         * The Set of Player usernames that can view this DynamicGameObject.
         */
        private readonly HashSet<string> players = new(); // TODO more appropriate type?

        /**
		 * Creates the DynamicGameObject.
		 *
		 * @param world The {@link World} containing the DynamicGameObject.
		 * @param id The id of the DynamicGameObject
		 * @param position The {@link Position} of the DynamicGameObject.
		 * @param type The type of the DynamicGameObject.
		 * @param orientation The orientation of the DynamicGameObject.
		 * @param alwaysVisible The flag indicates whether or not this DynamicGameObject is visible to every player.
		 */
        private DynamicGameObject(int id, Position position, int type, int orientation, bool alwaysVisible)
            : base(id, position, type, orientation)
        {
            this.alwaysVisible = alwaysVisible;
        }

        /**
		 * Adds this DynamicGameObject to the view of the specified {@link Player}.
		 *
		 * @param player The Player.
		 * @return {@code true} if this DynamicGameObject was not already visible to the specified Player.
		 */
        public bool AddTo(Player player)
        {
            return players.Add(player.Username);
        }


        public override EntityType EntityType => EntityType.Dynamic_Object;

        /**
		 * Removes this DynamicGameObject from the view of the specified {@link Player}.
		 *
		 * @param player The Player.
		 * @return {@code true} if this DynamicGameObject was visible to the specified Player.
		 */
        public bool RemoveFrom(Player player)
        {
            return players.Remove(player.Username);
        }

        public override bool ViewableBy(Player player, IWorld world)
        {
            return alwaysVisible || players.Contains(player.Username);
        }
    }
}
