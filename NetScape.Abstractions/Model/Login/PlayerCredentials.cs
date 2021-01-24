namespace NetScape.Abstractions.Model.Login
{    
    /// <summary>
    /// Holds the player credentials for a player
    /// </summary>
    public sealed record PlayerCredentials
    {

        /// <summary>
        /// Gets or sets the encoded username.
        /// </summary>
        /// <value>
        /// The encoded username.
        /// </value>
        public long EncodedUsername { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the uid.
        /// </summary>
        /// <value>
        /// The uid.
        /// </value>
        public int Uid { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the username hash.
        /// </summary>
        /// <value>
        /// The username hash.
        /// </value>
        public string UsernameHash { get; set; }

        /// <summary>
        /// Gets or sets the host address.
        /// </summary>
        /// <value>
        /// The host address.
        /// </value>
        public string HostAddress { get; set; }
    }
}
