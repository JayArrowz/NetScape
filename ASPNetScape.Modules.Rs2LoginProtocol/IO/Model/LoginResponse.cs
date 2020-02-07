namespace ASPNetScape.Modules.LoginProtocolThreeOneSeven.IO.Model
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
        private readonly bool _flagged;

        /**
         * The rights level.
         */
        private readonly int _rights;

        /**
         * The login status.
         */
        private readonly int _status;

        /**
         * Creates the login response.
         *
         * @param status The login status.
         * @param rights The rights level.
         * @param flagged The flagged flag.
         */
        public LoginResponse(int status, int rights, bool flagged)
        {
            this._status = status;
            this._rights = rights;
            this._flagged = flagged;
        }

        /**
         * Gets the rights level.
         *
         * @return The rights level.
         */
        public int GetRights()
        {
            return _rights;
        }

        /**
         * Gets the status.
         *
         * @return The status.
         */
        public LoginStatus GetStatus()
        {
            return (LoginStatus)_status;
        }

        /**
         * Checks if the player should be flagged.
         *
         * @return The flagged flag.
         */
        public bool IsFlagged()
        {
            return _flagged;
        }
    }
}
