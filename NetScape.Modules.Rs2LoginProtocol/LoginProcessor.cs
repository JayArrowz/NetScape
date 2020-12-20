using NetScape.Abstractions.Interfaces.Login;
using NetScape.Abstractions.Login.Model;

namespace NetScape.Modules.LoginProtocol
{
    public class LoginProcessor : ILoginProcessor<LoginStatus>
    {
        public LoginResponse<LoginStatus> Process(LoginRequest request)
        {
            return new LoginResponse<LoginStatus>
            {
                 Status = LoginStatus.StatusOk
            };
        }
    }
}
