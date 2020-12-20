namespace NetScape.Abstractions.Login.Model
{
    /**
     * Represents a login response.
     *
     * @author Graham
     */
    public sealed class LoginResponse<T>
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
        public T Status { get; set; }

    }
}
