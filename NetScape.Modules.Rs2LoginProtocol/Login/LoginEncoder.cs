using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using NetScape.Abstractions.Login.Model;

namespace NetScape.Modules.LoginProtocol.Login
{
    public class LoginEncoder : MessageToByteEncoder<LoginResponse<LoginStatus>>
    {
        protected override void Encode(IChannelHandlerContext context, LoginResponse<LoginStatus> message, IByteBuffer output)
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
