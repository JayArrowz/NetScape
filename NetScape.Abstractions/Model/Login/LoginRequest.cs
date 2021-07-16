using DotNetty.Transport.Channels;
using System;
using System.Threading.Tasks;
using NetScape.Abstractions.Util;

namespace NetScape.Abstractions.Model.Login
{
    public record LoginRequest<TRes>
    {
        public int[] ArchiveCrcs { get; set; }

        public int ClientVersion { get; set; }

        public PlayerCredentials Credentials { get; set; }

        public bool LowMemory { get; set; }

        public IsaacRandomPair RandomPair { get; set; }

        public bool Reconnecting { get; set; }

        public int ReleaseNumber { get; set; }
        public TRes Result { get; set; }

        /// <summary>
        /// Called on response of request <seealso cref="LoginProcessor.ProcessLoginsAsync"/>
        /// </summary>
        public Func<TRes, Task> OnResult { get; set; }
    }
}
