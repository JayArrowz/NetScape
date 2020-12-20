namespace NetScape.Modules.LoginProtocol.IO.Model
{
    /**
     * Represents a login response.
     *
     * @author Graham
     */
    public sealed class LoginResponse
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
        public LoginStatus Status { get; set; }

    }
}
