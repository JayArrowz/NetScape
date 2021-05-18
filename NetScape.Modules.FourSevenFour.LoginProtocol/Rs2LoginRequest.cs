using NetScape.Abstractions.Model.Login;
using System;
using System.Threading.Tasks;

namespace NetScape.Modules.FourSevenFour.LoginProtocol
{
    public record Rs2LoginRequest : LoginRequest<Rs2LoginResponse>
    {
    }
}
