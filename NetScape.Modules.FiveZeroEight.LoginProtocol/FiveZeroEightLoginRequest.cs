using NetScape.Abstractions.Model.Login;
using System;
using System.Threading.Tasks;

namespace NetScape.Modules.FiveZeroEight.LoginProtocol
{
    public record FiveZeroEightLoginRequest : LoginRequest<FiveZeroEightLoginResponse>
    {
    }
}
