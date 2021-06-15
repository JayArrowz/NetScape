using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Serilog;
using System;

namespace NetScape.Modules.Server
{
    public class GameServerChannelHandler : SimpleChannelInboundHandler<IByteBuffer>
    {
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Log.Logger.Error(exception, nameof(GameServerChannelHandler));
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, IByteBuffer msg)
        {
            msg.Retain();
            ctx.FireChannelRead(msg);
        }
    }
}
