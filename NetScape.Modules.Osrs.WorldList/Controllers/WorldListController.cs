using DotNetty.Buffers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetScape.Abstractions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NetScape.Modules.Osrs.WorldList.Controllers
{
    [ApiController]
    [Route("/")]
    public class WorldListController : ControllerBase
    {
        private readonly ILogger<WorldListController> _logger;

        public WorldListController(ILogger<WorldListController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("{**catchAll}")]
        public HttpResponseMessage Get()
        {
            var buf = Unpooled.Buffer();
            buf.WriteShort(1);
            var mask = 0;
            buf.WriteShort(0);
            buf.WriteShort(mask);
            buf.WriteString("127.0.0.1", Encoding.ASCII);
            buf.WriteString("None", Encoding.ASCII); //activity
            buf.WriteByte(1); //location
            buf.WriteShort(1); //players

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(buf.GetBytes())
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return result;
        }
    }
}
