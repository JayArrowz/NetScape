using NetScape.Abstractions.Model;
using NetScape.Abstractions.Model.Game;
using NetScape.Modules.World;
using System.Collections.Generic;

namespace NetScape.Modules.Region
{
    public class DynamicGameObject : GameObject
    {
        /// <summary>
        /// Creates a DynamicGameObject that is visible only to <see cref="Model.Game.Player"/> specified later.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="position">The position.</param>
        /// <param name="type">The type.</param>
        /// <param name="orientation">The orientation.</param>
        /// <returns>The DynamicGameObject</returns>
        public static DynamicGameObject CreateLocal(int id, Position position, int type, int orientation)
        {
            return new DynamicGameObject(id, position, type, orientation, false);
        }

        /// <summary>
        /// Creates a DynamicGameObject that is always visible.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="position">The position.</param>
        /// <param name="type">The type.</param>
        /// <param name="orientation">The orientation.</param>
        /// <returns></returns>
        public static DynamicGameObject CreatePublic(int id, Position position, int type,
                int orientation)
        {
            return new DynamicGameObject(id, position, type, orientation, true);
        }

        /// <summary>
        /// The flag indicating whether or not this DynamicGameObject is visible to every player.
        /// </summary>
        private readonly bool _alwaysVisible;

        /// <summary>
        /// The Set of Player usernames that can view this DynamicGameObject.
        /// </summary>
        private readonly HashSet<string> players = new(); // TODO more appropriate type?

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicGameObject"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="position">The position.</param>
        /// <param name="type">The type.</param>
        /// <param name="orientation">The orientation.</param>
        /// <param name="alwaysVisible">if set to <c>true</c> [always visible].</param>
        private DynamicGameObject(int id, Position position, int type, int orientation, bool alwaysVisible)
            : base(id, position, type, orientation)
        {
            this._alwaysVisible = alwaysVisible;
        }

        /// <summary>
        /// Adds this DynamicGameObject to the view of the specified player
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>if set to <c>true</c> [if this DynamicGameObject was not already visible to the specified Player]</returns>      
        public bool AddTo(Player player)
        {
            return players.Add(player.Username);
        }


        public override EntityType EntityType => EntityType.Dynamic_Object;

        /// <summary>
        /// Removes this DynamicGameObject from the view of the specified Player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns><c>true</c> [if this DynamicGameObject was visible to the specified Player.]</returns>    
        public bool RemoveFrom(Player player)
        {
            return players.Remove(player.Username);
        }

        public override bool ViewableBy(Player player, IWorld world)
        {
            return _alwaysVisible || players.Contains(player.Username);
        }
    }
}
