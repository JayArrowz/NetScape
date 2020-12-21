using DotNetty.Transport.Channels;
using NetScape.Abstractions.IO.Util;

namespace NetScape.Abstractions.Model.Login
{
    /**
     * Represents a login request.
     *
     * @author Graham
     */
    public record LoginRequest<TRes>
    {
        public int[] ArchiveCrcs { get; set; }

        public int ClientVersion { get; set; }

        public PlayerCredentials Credentials { get; set; }

        public bool LowMemory { get; set; }

        public IsaacRandomPair RandomPair { get; set; }

        public bool Reconnecting { get; set; }

        public int ReleaseNumber { get; set; }
        public TRes Result { get; set; }
    }
}
