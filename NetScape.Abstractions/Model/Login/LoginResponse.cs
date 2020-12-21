namespace NetScape.Abstractions.Login.Model
{
    /**
     * Represents a login response.
     *
     * @author Graham
     */
    public class LoginResponse<TStatus>
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
        public TStatus Status { get; set; }

    }
}
