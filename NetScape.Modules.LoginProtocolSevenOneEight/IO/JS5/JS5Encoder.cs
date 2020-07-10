using System.Collections.Generic;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;

namespace ASPNetScape.Modules.SevenOneEight.LoginProtocol.IO.JS5
{
    public sealed class JS5Encoder : MessageToMessageEncoder<IByteBuffer>
    {
        public static readonly
            AttributeKey<IntWrapper> JS5_Encoder_Key = AttributeKey<IntWrapper>.ValueOf(nameof(JS5Encoder));

        protected override void Encode(IChannelHandlerContext context, IByteBuffer message, List<object> output)
        {
            var encoderValue = context.Channel.GetAttribute(JS5_Encoder_Key);
            if (encoderValue.Get() != null)
            {
                var encode = encoderValue.Get();
                if (encode.Value != 0)
                {
                    for (int i = 0; i < message.ReadableBytes; i++)
                    {
                        message.SetByte(i, message.GetByte(i) ^ encode.Value);
                    }
                }
            }
            output.Add(message);
        }
    }

    public class IntWrapper
    {
        public int Value { get; set; }
    }
}
