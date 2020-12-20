namespace NetScape.Abstractions.Login.Model
{
    /**
     * Represents a login response.
     *
     * @author Graham
     */
    public sealed class LoginResponse<TResponse>
    {

        /**
         * The flagged flag.
         */
        public bool Flagged { get; set; }

        /**
         * The rights level.
         */
        public int Rights { get; set; }

        /**
         * The login status.
         */
        public TResponse Status { get; set; }

    }
}
