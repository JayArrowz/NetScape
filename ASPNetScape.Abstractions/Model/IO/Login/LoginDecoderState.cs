namespace ASPNetScape.Modules.LoginProtocolThreeOneSeven.IO.Model
{
    public enum LoginDecoderState
    {

        /**
         * The login handshake state will wait for the username hash to be received. Once it is, a server session key will
         * be sent to the client and the state will be set to the login header state.
         */
        LoginHandshake,

        /**
         * The login header state will wait for the login type and payload length to be received. These are saved, and then
         * the state will be set to the login payload state.
         */
        LoginHeader,

        /**
         * The login payload state will wait for all login information (such as client release number, username and
         * password).
         */
        LoginPayload

    }
}
