using NetScape.Abstractions.Model.Login;
using System;
using System.Threading.Tasks;

namespace NetScape.Modules.FourSevenFour.LoginProtocol
{
    public record Rs2LoginRequest : LoginRequest<Rs2LoginResponse>
    {
        /// <summary>
        /// Called on response of request <seealso cref="LoginProcessor.ProcessLoginsAsync"/>
        /// </summary>
        public Func<Rs2LoginResponse, Task> OnResult { get; set; }
    }
}
