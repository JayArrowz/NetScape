namespace NetScape.Abstractions.Model.Game
{
    /// <summary>
    /// Represents a type of <see cref="Entity"/>
    /// </summary>
    public enum EntityType
    {
        /// <summary>
        /// A GameObject that is loaded dynamically, usually for specific Players.
        /// </summary>
        Dynamic_Object,

        /// <summary>
        /// An Item that is positioned on the ground.
        /// </summary>
        Ground_Item,

        /// <summary>
        /// A NPC
        /// </summary>
        Npc,

        /// <summary>
        /// A player
        /// </summary>
        Player,

        /// <summary>
        /// A projectile (e.g. an arrow).
        /// </summary>
        Projectile,

        /// <summary>
        /// A GameObject that is loaded statically (i.e. from the game resources) at start-up.
        /// </summary>
        Static_Object
    }
}
