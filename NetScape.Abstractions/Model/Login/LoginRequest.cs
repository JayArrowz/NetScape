using NetScape.Abstractions.IO;
using NetScape.Abstractions.Model.IO.Login;

namespace NetScape.Abstractions.Login.Model
{
    /**
     * Represents a login request.
     *
     * @author Graham
     */
    public sealed class LoginRequest
    {
        public int[] ArchiveCrcs { get; set; }

        public int ClientVersion { get; set; }

        public PlayerCredentials Credentials { get; set; }

        public bool LowMemory { get; set; }

        public IsaacRandomPair RandomPair { get; set; }

        public bool Reconnecting { get; set; }

        public int ReleaseNumber { get; set; }
    }
}
