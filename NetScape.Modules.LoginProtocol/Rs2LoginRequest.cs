using NetScape.Abstractions.Model.Login;
using System;
using System.Threading.Tasks;

namespace NetScape.Modules.LoginProtocol
{
    public record Rs2LoginRequest : LoginRequest<Rs2LoginResponse>
    {
        public Func<Rs2LoginResponse, Task> OnResult { get; set; }
    }
}
