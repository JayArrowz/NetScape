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
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<GeneratorParams>(fileText);
                })
                .ToList();
            var stringBuilder = new StringBuilder();
            foreach (var param in generatorParams)
            {
                foreach (var message in param.Messages)
                {
                    var strBuilder = new StringBuilder();
                    var fieldsBuilder = new StringBuilder();
                    var getMethod = new StringBuilder();
                    strBuilder.Append($@"namespace {param.Namespace}.Generated {{
using DotNetty.Buffers;
using NetScape.Modules.Messages.Builder;
    public class {message.Name} {{
{{Fields}}        
        public MessageFrame ToMessageFrame(IByteBufferAllocator alloc) {{
{{GetMethod}}
        }}
    }}
}}");

                    getMethod.AppendLine($"            var bldr = new MessageFrameBuilder(alloc, {message.Id}, MessageFrame.MessageType.{message.FrameType});");
                    foreach (var fields in message.Params)
                    {
                        getMethod.AppendLine($"            bldr.Put(MessageType.{fields.Type}, DataOrder.{fields.Order}, DataTransformation.{fields.Transform}, {fields.Name});");
                        fieldsBuilder.AppendLine($"        public {MessageTypeToString(fields.Type)} {fields.Name} {{ get; set; }}");
                    }
                    getMethod.Append("            return bldr.ToMessageFrame();");
                    strBuilder.Replace("{Fields}", fieldsBuilder.ToString());
                    strBuilder.Replace("{GetMethod}", getMethod.ToString());
                    context.AddSource($"{message.Name}", SourceText.From(strBuilder.ToString(), Encoding.UTF8));
                }
            }

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
