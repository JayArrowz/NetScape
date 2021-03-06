﻿using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Serilog;
using System.Collections.Generic;

namespace NetScape.Modules.FourSevenFour.LoginProtocol.Handlers
{
    public class JS5Decoder : ByteToMessageDecoder
    {
        private int _encryptionKey = 0;
        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            if (input.ReadableBytes > 0 && input.IsReadable())
            {
                input.MarkReaderIndex();
                int opcode = input.ReadByte();

                Log.Logger.Debug("File Request: Opcode: {0}", opcode);
                if (opcode == 0 || opcode == 1)
                {
                    if (input.ReadableBytes < 3)
                    {
                        input.ResetReaderIndex();
                        return;
                    }
                    int index = input.ReadByte();
                    int file = input.ReadUnsignedShort();
                    bool priority = opcode == 0;
                    Log.Logger.Debug("File Request: Index: {0}, File: {1} Priority: {2}", index, file, priority);
                    _ = context.Channel.WriteAndFlushAsync(new JS5Request(index, file, priority, _encryptionKey));
                }
                else if (opcode == 2 || opcode == 3 || opcode == 6)
                {
                    input.SkipBytes(3);
                }
                else if (opcode == 4)
                {
                    _encryptionKey = input.ReadByte();
                    if (input.ReadShort() != 0)
                    {
                        _ = context.Channel.DisconnectAsync();
                    }
                }
                else
                {
                    Log.Logger.Warning("Unknown JS5 Opcode: {0} Size: {1}", opcode, input.ReadableBytes);
                }
            }
        }
    }
}
