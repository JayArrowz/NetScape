using NetScape.Abstractions.Util;

namespace NetScape.Abstractions.Interfaces.Messages
{
    /// <summary>
    /// A handler which has a cipher <seealso cref="IsaacRandom"/>
    /// </summary>
    public interface ICipheredHandler
    {
        /// <summary>
        /// Gets or sets the cipher.
        /// </summary>
        /// <value>
        /// The cipher.
        /// </value>
        IsaacRandom Cipher { get; set; }
    }
}
