namespace NetScape.Abstractions.Model.IO.Login
{
    /**
     * Holds the credentials for a player.
     *
     * @author Graham
     */
    public sealed record PlayerCredentials
    {

        /**
         * The player's username encoded as a long.
         */
        public long EncodedUsername { get; set; }

        /**
         * The player's password.
         */
        public string Password { get; set; }

        /**
         * The computer's unique identifier.
         */
        public int Uid { get; set; }

        /**
         * The player's username.
         */
        public string Username { get; set; }

        /**
         * The hash of the player's username.
         */
        public string UsernameHash { get; set; }

        /**
         * The Player's host address, represented as a string.
         */
        public string HostAddress { get; set; }
    }
}
