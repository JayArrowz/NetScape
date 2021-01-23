using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using NetScape.Modules.Messages.Builder;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace NetScape.Messages.Generator
{
    [Generator]
    public class MessageGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            // find anything that matches our files
            var generatorParams = context.AdditionalFiles.Where(at => at.Path.EndsWith("messages.json"))
                .Select(t =>
                {
                    var fileText = File.ReadAllText(t.Path);
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<FrameParameters>(fileText);
                })
                .ToList();
            var stringBuilder = new StringBuilder();
            foreach (var param in generatorParams)
            {
                var isEncoder = param.Type == GeneratorMessageType.Encoder;
                foreach (var message in param.Messages)
                {
                    if(isEncoder)
                    {
                        context = GenerateEncoders(message, param, context);
                    }
                }
            }

        }

        private GeneratorExecutionContext GenerateEncoders(GeneratorData message, FrameParameters param, GeneratorExecutionContext context)
        {
            var strBuilder = new StringBuilder();
            var fieldsBuilder = new StringBuilder();
            var getMethod = new StringBuilder();
            AppendEncoderTemplate(strBuilder, param.Namespace, message.Name);

            getMethod.AppendLine($"            var bldr = new MessageFrameBuilder(alloc, {message.Id}, FrameType.{message.FrameType});");
            foreach (var fields in message.Params)
            {
                getMethod.AppendLine($"            bldr.Put(MessageType.{fields.Type}, DataOrder.{fields?.Order ?? DataOrder.Big}, DataTransformation.{fields?.Transform ?? DataTransformation.None}, {fields.Name});");
                fieldsBuilder.AppendLine($"        public {MessageTypeToString(fields.Type)} {fields.Name} {{ get; set; }}");
            }
            getMethod.Append("            return bldr.ToMessageFrame();");
            strBuilder.Replace("{Fields}", fieldsBuilder.ToString());
            strBuilder.Replace("{GetMethod}", getMethod.ToString());
            context.AddSource($"{message.Name}.SourceGenerated.cs", SourceText.From(strBuilder.ToString(), Encoding.UTF8));
            return context;
        }

        private void AppendEncoderTemplate(StringBuilder strBuilder, string generatedNameSpace, string className)
        {
            strBuilder.Append($@"namespace {generatedNameSpace} {{
using DotNetty.Buffers;
using NetScape.Modules.Messages.Builder;
using NetScape.Modules.Messages.Encoders;
    public partial class {className} : IOutMessage<MessageFrame> {{
{{Fields}}        
        public MessageFrame ToMessage(IByteBufferAllocator alloc) {{
{{GetMethod}}
        }}
    }}
}}");
        }

        private string MessageTypeToString(MessageType type)
        {
            switch (type)
            {
                case MessageType.Byte:
                    return "byte";
                case MessageType.Int:
                    return "int";
                case MessageType.Long:
                    return "long";
                case MessageType.Short:
                    return "short";
                case MessageType.TriByte:
                    return "int";
            }
            return string.Empty;
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }

    }
}
