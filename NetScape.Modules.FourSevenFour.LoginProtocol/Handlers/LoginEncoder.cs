using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using NetScape.Abstractions.Model.Login;

namespace NetScape.Modules.FourSevenFour.LoginProtocol.Handlers
{
    public class LoginEncoder : MessageToByteEncoder<LoginResponse<LoginStatus>>
    {
        protected override void Encode(IChannelHandlerContext context, LoginResponse<LoginStatus> message, IByteBuffer output)
        {
            output.WriteByte((int)message.Status);

            if (message.Status == LoginStatus.StatusOk)
            {
                output.WriteByte(message.Rights);
                output.WriteByte(message.Flagged ? 1 : 0);
                output.WriteShort(message.Player.Index);
                output.WriteByte(1);
            }
        }
    }
}
