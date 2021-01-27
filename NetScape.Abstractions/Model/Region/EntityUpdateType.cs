namespace NetScape.Abstractions.Model.Region
{
    /// <summary>
    /// <author>Major</author>
    /// </summary>
    public enum EntityUpdateType
    {
        /// <summary>
        ///  The add type, when an Entity has been added to a <see cref="Interfaces.Region.IRegion"/>
        /// </summary>
        Add,

        /// <summary>
        /// The remove type, when an Entity has been removed from a <see cref="Interfaces.Region.IRegion"/>
        /// </summary>
        Remove
    }
}
