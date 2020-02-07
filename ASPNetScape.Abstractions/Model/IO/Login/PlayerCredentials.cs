using System;
using System.Collections.Generic;
using System.Text;

namespace ASPNetScape.Abstractions.Model.IO.Login
{
    /**
     * Holds the credentials for a player.
     *
     * @author Graham
     */
    public sealed class PlayerCredentials
    {

        /**
         * The player's username encoded as a long.
         */
        private readonly long _encodedUsername;

        /**
         * The player's password.
         */
        private string _password;

        /**
         * The computer's unique identifier.
         */
        private readonly int _uid;

        /**
         * The player's username.
         */
        private readonly string _username;

        /**
         * The hash of the player's username.
         */
        private readonly int _usernameHash;

        /**
         * The Player's host address, represented as a string.
         */
        private readonly string _hostAddress;

        /**
         * Creates a new {@link PlayerCredentials} object with the specified name, password and uid.
         *
         * @param username The player's username.
         * @param password The player's password.
         * @param usernameHash The hash of the player's username.
         * @param uid The computer's uid.
         * @param hostAddress The Player's connecting host address.
         */
        public PlayerCredentials(string username, string password, int usernameHash, int uid, string hostAddress)
        {
            this._username = username;
            //encodedUsername = NameUtil.encodeBase37(username);
            this._password = password;
            this._usernameHash = usernameHash;
            this._uid = uid;
            this._hostAddress = hostAddress;
        }

        /**
         * Gets the player's username encoded as a long.
         *
         * @return The username as encoded by {@link NameUtil#encodeBase37(string)}.
         */
        public long GetEncodedUsername()
        {
            return _encodedUsername;
        }

        /**
         * Sets the player's password
         *
         * @param password The player's new password
         */
        public void SetPassword(string password)
        {
            this._password = password;
        }

        /**
         * Gets the player's password.
         *
         * @return The player's password.
         */
        public string GetPassword()
        {
            return _password;
        }

        /**
         * Gets the computer's uid.
         *
         * @return The computer's uid.
         */
        public int GetUid()
        {
            return _uid;
        }

        /**
         * Gets the player's username.
         *
         * @return The player's username.
         */
        public string GetUsername()
        {
            return _username;
        }

        /**
         * Gets the username hash.
         *
         * @return The username hash.
         */
        public int GetUsernameHash()
        {
            return _usernameHash;
        }

        /**
         * Gets the Player's connecting host address.
         *
         * @return The Player's host address, represented as a string.
         */
        public string GetHostAddress()
        {
            return _hostAddress;
        }


        //public int hashCode()
        //{
        //    return Long.hashCode(encodedUsername);
        //}

        //@Override
        //public boolean equals(Object obj)
        //{
        //    if (obj instanceof PlayerCredentials) {
        //        PlayerCredentials other = (PlayerCredentials)obj;
        //        return encodedUsername == other.encodedUsername;
        //    }

        //    return false;
        //}

    }
}
