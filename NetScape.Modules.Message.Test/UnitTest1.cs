using Google.Protobuf;
using NetScape.Modules.Messages.Models;
using Xunit;

namespace NetScape.Modules.Message.Test
{
    public class ProtoEncoderTests
    {
        [Fact]
        public void Test1()
        {
            var messageTest = new ThreeOneSevenDecoderMessages.Types.ArrowKeyMessage() as IMessage;

        }
    }
}
