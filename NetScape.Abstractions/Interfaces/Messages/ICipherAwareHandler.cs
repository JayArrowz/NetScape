using NetScape.Abstractions.IO.Util;
using NetScape.Abstractions.Util;

namespace NetScape.Abstractions.Interfaces.Messages
{
    /// <summary>
    /// A handler which has a cipher <seealso cref="IsaacRandom"/>
    /// </summary>
    public interface ICipherAwareHandler
    {
        /// <summary>
        /// Gets or sets the cipher pair.
        /// </summary>
        /// <value>
        /// The cipher pair.
        /// </value>
        IsaacRandomPair CipherPair { get; set; }
    }
}
