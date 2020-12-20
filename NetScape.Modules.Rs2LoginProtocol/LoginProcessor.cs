using NetScape.Abstractions.Interfaces.Login;
using NetScape.Abstractions.Login.Model;
using Serilog;
using System.Threading.Tasks;

namespace NetScape.Modules.LoginProtocol
{
    public class LoginProcessor : ILoginProcessor<LoginStatus>
    {
        private ILogger _logger;
        public LoginProcessor(ILogger logger)
        {
            _logger = logger;
        }

        public Task<LoginResponse<LoginStatus>> ProcessAsync(LoginRequest request)
        {
            var password = request.Credentials.Password;
            var username = request.Credentials.Username;
            _logger.Information("Pending login from {0}", username);

            if (password.Length < 4 || password.Length > 20 || string.IsNullOrEmpty(username) || username.Length > 12)
            {
                _logger.Information("Username ('{0}') or password did not pass validation.", username);
                return Task.FromResult(new LoginResponse<LoginStatus> { Status = LoginStatus.StatusInvalidCredentials });
            }

            return Task.FromResult(new LoginResponse<LoginStatus>
            {
                Status = LoginStatus.StatusOk
            });
        }
    }
}
