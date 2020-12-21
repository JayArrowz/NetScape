using NetScape.Abstractions.Model.Login;

namespace NetScape.Modules.LoginProtocol
{
    public record Rs2LoginRequest : LoginRequest<Rs2LoginResponse>
    {
    }

    public class Rs2LoginResponse : LoginResponse<LoginStatus>
    {
    }
}
