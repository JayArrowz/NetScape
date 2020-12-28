using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using NetScape.Abstractions;
using NetScape.Abstractions.Model.Game;
using NetScape.Modules.World;
using Serilog;
using System;
using System.Threading.Tasks;

namespace NetScape.Modules.Messages
{
    public class MessageChannelHandler : SimpleChannelInboundHandler<IByteBuffer>
    {
        private readonly ILogger _logger;
        private readonly IWorld _world;

        public MessageChannelHandler(ILogger logger, IWorld world)
        {
            _logger = logger;
            _world = world;
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, IByteBuffer msg)
        {
            msg.Retain();
            ctx.FireChannelRead(msg);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            var player = context.GetAttribute(Constants.PlayerAttributeKey).GetAndRemove();
            _world.Remove(player);
            _logger.Information("Player: {0} disconnected Addr: {1}", player.Username, context.Channel.RemoteAddress);
            base.ChannelInactive(context);
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            context.CloseAsync();
            _logger.Error(exception, nameof(ExceptionCaught));
            base.ExceptionCaught(context, exception);
        }
    }
}
