using NetScape.Abstractions.Model.Game;

namespace NetScape.Abstractions.Model.Login
{
    /**
     * Represents a login response.
     * @author Graham
     */
    public class LoginResponse<TStatus>
    {

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="LoginResponse{TStatus}"/> is flagged.
        /// </summary>
        /// <value>
        ///   <c>true</c> if flagged; otherwise, <c>false</c>.
        /// </value>
        public bool Flagged { get; set; }

        /// <summary>
        /// Gets or sets the rights.
        /// </summary>
        /// <value>
        /// The rights.
        /// </value>
        public int Rights { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public TStatus Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Player"/> is created.
        /// </summary>
        /// <value>
        ///   <c>true</c> if created; otherwise, <c>false</c>.
        /// </value>
        public bool Created { get; set; }

        /// <summary>
        /// Gets or sets the player.
        /// </summary>
        /// <value>
        /// The player.
        /// </value>
        public Player Player { get; set; }
    }
}
