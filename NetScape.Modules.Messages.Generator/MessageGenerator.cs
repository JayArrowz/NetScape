using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using NetScape.Modules.Messages.Builder;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace NetScape.Messages.Generator
{
    [Obsolete]
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
                    if (isEncoder)
                    {
                        context = GenerateEncoders(message, param, context);
                    }
                    else
                    {
                        context = GenerateDecoders(message, param, context);
                    }
                }
            }

        }

        private GeneratorExecutionContext GenerateDecoders(GeneratorData message, FrameParameters param, GeneratorExecutionContext context)
        {
            var strBuilder = new StringBuilder();
            var fieldsBuilder = new StringBuilder();
            var getMethod = new StringBuilder();
            AppendDecoderTemplate(strBuilder, message, param.Namespace, message.Name);
            getMethod.AppendLine("            var reader = new MessageFrameReader(frame);");
            foreach (var fields in message.Params)
            {
                var fieldName = fields.Name;
                var msgType = MessageTypeToString(fields.Type, fields.Signed);
                fieldsBuilder.AppendLine($"        public {msgType} {fields.Name} {{ get; set; }}");
                var readMethod = fields.Signed ? "GetSigned" : "GetUnsigned";
                getMethod.AppendLine($"            var {fieldName.ToLower()} = ({msgType}) reader.{readMethod}(MessageType.{fields.Type}, DataOrder.{fields.Order ?? DataOrder.Big}, DataTransformation.{fields.Transform ?? DataTransformation.None});");
            }

            var varToFieldMap = message.Params.Select(t => $"                {t.Name} = {t.Name.ToLower()},").ToList();
            getMethod.AppendLine($"            return new {message.Name} {{");
            foreach (var varToF in varToFieldMap)
            {
                getMethod.AppendLine(varToF);
            }
            getMethod.AppendLine("            };");
            strBuilder.Replace("{Fields}", fieldsBuilder.ToString());
            strBuilder.Replace("{GetMethod}", getMethod.ToString());
            context.AddSource($"{message.Name}.SourceGenerated.cs", SourceText.From(strBuilder.ToString(), Encoding.UTF8));
            return context;
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
                fieldsBuilder.AppendLine($"        public {MessageTypeToString(fields.Type, fields.Signed)} {fields.Name} {{ get; set; }}");
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
    public partial class {className} : IEncoderMessage<MessageFrame> {{
{{Fields}}        
        public MessageFrame ToMessage(IByteBufferAllocator alloc) {{
{{GetMethod}}
        }}
    }}
}}");
        }

        private void AppendDecoderTemplate(StringBuilder strBuilder, GeneratorData data, string generatedNameSpace, string className)
        {
            strBuilder.Append($@"namespace {generatedNameSpace} {{
using NetScape.Abstractions.Model.Game;
using NetScape.Modules.Messages.Builder;
using NetScape.Abstractions.Model.Messages;
    public class {className} : DecoderMessage {{
{{Fields}}
    }}
    public partial class {className}Decoder : MessageDecoderBase<{className}> {{
        public override int[] Ids {{ get; }} = new int[] {{{data.Id}}};
        public override FrameType FrameType {{ get; }} = FrameType.{data.FrameType};
     
        protected override {className} Decode(Player player, MessageFrame frame) {{
{{GetMethod}}
        }}
    }}
}}");
        }

        private string MessageTypeToString(MessageType type, bool signed)
        {
            switch (type)
            {
                case MessageType.Byte:
                    return "byte";
                case MessageType.Int:
                    return signed ? "int" : "uint";
                case MessageType.Long:
                    return signed ? "long" : "ulong";
                case MessageType.Short:
                    return signed ? "short" : "ushort";
                case MessageType.TriByte:
                    return signed ? "int" : "uint";
            }
            return string.Empty;
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }

    }
}
