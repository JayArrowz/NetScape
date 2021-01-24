using NetScape.Abstractions.Interfaces.Messages;

namespace NetScape.Abstractions.Interfaces.Region
{
    public interface IRegionUpdateOperation
    {
        /// <summary>
        /// Gets a <see cref="RegionUpdateMessage"/> that would counteract the effect of this UpdateOperation.
        /// </summary>
        /// <returns>The RegionUpdateMessage</returns>
        RegionUpdateMessage Inverse();

        /// <summary>
        /// Returns this UpdateOperation as a RegionUpdateMessage.
        /// </summary>
        /// <returns>RegionUpdateMessage</returns>
        RegionUpdateMessage ToMessage();
    }
}