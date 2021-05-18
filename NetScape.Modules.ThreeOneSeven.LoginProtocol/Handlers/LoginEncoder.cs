using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using NetScape.Abstractions.Model.Login;

namespace NetScape.Modules.ThreeOneSeven.LoginProtocol.Handlers
{
    public class LoginEncoder : MessageToByteEncoder<LoginResponse<ThreeOneSevenLoginStatus>>
    {
        protected override void Encode(IChannelHandlerContext context, LoginResponse<ThreeOneSevenLoginStatus> message, IByteBuffer output)
        {
            output.WriteByte((int)message.Status);

            if (message.Status == ThreeOneSevenLoginStatus.StatusOk)
            {
                output.WriteByte(message.Rights);
                output.WriteByte(message.Flagged ? 1 : 0);
            }
        }
    }
}
