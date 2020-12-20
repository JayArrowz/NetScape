using NetScape.Abstractions.Model.IO.Login;
using NetScape.Modules.LoginProtocol.IO.Model;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace NetScape.Modules.LoginProtocol.IO.Login
{
    public class LoginEncoder : MessageToByteEncoder<LoginResponse>
    {
        protected override void Encode(IChannelHandlerContext context, LoginResponse message, IByteBuffer output)
        {
            output.WriteByte((int) message.Status);

            if (message.Status == LoginStatus.StatusOk)
            {
                output.WriteByte(message.Rights);
                output.WriteByte(message.Flagged ? 1 : 0);
            }
        }
    }
}
