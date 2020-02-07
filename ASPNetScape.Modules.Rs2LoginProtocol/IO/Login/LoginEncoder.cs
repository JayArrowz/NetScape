using ASPNetScape.Abstractions.Model.IO.Login;
using ASPNetScape.Modules.LoginProtocolThreeOneSeven.IO.Model;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace ASPNetScape.Modules.LoginProtocolThreeOneSeven.IO.Login
{
    public class LoginEncoder : MessageToByteEncoder<LoginResponse>
    {
        protected override void Encode(IChannelHandlerContext context, LoginResponse message, IByteBuffer output)
        {
            output.WriteByte((int)message.GetStatus());

            if (message.GetStatus() == LoginStatus.StatusOk)
            {
                output.WriteByte(message.GetRights());
                output.WriteByte(message.IsFlagged() ? 1 : 0);
            }

        }
    }
}
