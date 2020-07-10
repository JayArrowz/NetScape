using NetScape.Abstractions.IO;
using NetScape.Abstractions.Model.IO.Login;

namespace NetScape.Modules.LoginProtocolThreeOneSeven.IO.Model
{
    /**
     * Represents a login request.
     *
     * @author Graham
     */
    public sealed class LoginRequest
    {
        /**
         * The archive CRCs.
         */
        private readonly int[] _archiveCrcs;

        /**
         * The version denoting whether the client has been modified or not.
         */
        private readonly int _clientVersion;

        /**
         * The player's credentials.
         */
        private readonly PlayerCredentials _credentials;

        /**
         * The low memory flag.
         */
        private readonly bool _lowMemory;

        /**
         * The pair of random number generators.
         */
        private readonly IsaacRandomPair _randomPair;

        /**
         * The reconnecting flag.
         */
        private readonly bool _reconnecting;

        /**
         * The release number.
         */
        private readonly int _releaseNumber;

        /**
         * Creates a login request.
         *
         * @param credentials The player credentials.
         * @param randomPair The pair of random number generators.
         * @param lowMemory The low memory flag.
         * @param reconnecting The reconnecting flag.
         * @param releaseNumber The release number.
         * @param archiveCrcs The archive CRCs.
         * @param clientVersion The client version.
         */
        public LoginRequest(PlayerCredentials credentials, IsaacRandomPair randomPair, bool lowMemory, bool reconnecting, int releaseNumber, int[] archiveCrcs, int clientVersion)
        {
            this._credentials = credentials;
            this._randomPair = randomPair;
            this._lowMemory = lowMemory;
            this._reconnecting = reconnecting;
            this._releaseNumber = releaseNumber;
            this._archiveCrcs = archiveCrcs;
            this._clientVersion = clientVersion;
        }

        /**
         * Gets the archive CRCs.
         *
         * @return The array of archive CRCs.
         */
        public int[] GetArchiveCrcs()
        {
            return _archiveCrcs;
        }

        /**
         * Gets the value denoting the client's (modified) version.
         *
         * @return The client version.
         */
        public int GetClientVersion()
        {
            return _clientVersion;
        }

        /**
         * Gets the player's credentials.
         *
         * @return The player's credentials.
         */
        public PlayerCredentials GetCredentials()
        {
            return _credentials;
        }

        /**
         * Gets the pair of random number generators.
         *
         * @return The pair of random number generators.
         */
        public IsaacRandomPair GetRandomPair()
        {
            return _randomPair;
        }

        /**
         * Gets the release number.
         *
         * @return The release number.
         */
        public int GetReleaseNumber()
        {
            return _releaseNumber;
        }

        /**
         * Checks if this client is in low memory mode.
         *
         * @return {@code true} if so, {@code false} if not.
         */
        public bool IsLowMemory()
        {
            return _lowMemory;
        }

        /**
         * Checks if this client is reconnecting.
         *
         * @return {@code true} if so, {@code false} if not.
         */
        public bool IsReconnecting()
        {
            return _reconnecting;
        }

    }
}
