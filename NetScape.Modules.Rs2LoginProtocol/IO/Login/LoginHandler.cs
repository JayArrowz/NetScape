using NetScape.Abstractions.Model.IO;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;

namespace NetScape.Modules.LoginProtocolThreeOneSeven.IO.Login
{
    public class LoginHandler : SimpleChannelInboundHandler<HandshakeType>
    {
        /**
         * The {@link Session} {@link AttributeKey}.
         */
        public static readonly AttributeKey<object> SESSION_KEY = AttributeKey<object>.ValueOf("session");

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
            base.ChannelReadComplete(context);
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, HandshakeType msg)
        {
            
        }
    }
}
