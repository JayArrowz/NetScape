using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using NetScape.Abstractions.Model.Login;

namespace NetScape.Modules.FiveZeroEight.LoginProtocol.Handlers
{
    public class LoginEncoder : MessageToByteEncoder<LoginResponse<FiveZeroEightLoginStatus>>
    {
        protected override void Encode(IChannelHandlerContext context, LoginResponse<FiveZeroEightLoginStatus> message, IByteBuffer output)
        {
            output.WriteByte((int)message.Status);

            if (message.Status == FiveZeroEightLoginStatus.StatusOk)
            {
                output.WriteByte(message.Rights);
                output.WriteByte(message.Flagged ? 1 : 0);
                output.WriteShort(message.Player.Index);
                output.WriteByte(1);
            }
        }
    }
}
