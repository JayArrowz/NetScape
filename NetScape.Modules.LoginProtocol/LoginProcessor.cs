using NetScape.Abstractions.FileSystem;
using NetScape.Abstractions.Interfaces.Login;
using NetScape.Abstractions.Login.Model;
using Serilog;
using System.Threading.Tasks;

namespace NetScape.Modules.LoginProtocol
{
    public class LoginProcessor : ILoginProcessor<LoginStatus>
    {
        private ILogger _logger;
        private readonly IPlayerSerializer _playerSerializer;

        public LoginProcessor(ILogger logger, IPlayerSerializer playerSerializer)
        {
            _logger = logger;
            _playerSerializer = playerSerializer;
        }

        public async Task<LoginResponse<LoginStatus>> ProcessAsync(LoginRequest request)
        {
            var password = request.Credentials.Password;
            var username = request.Credentials.Username;
            _logger.Information("Pending login from {0}", username);

            if (password.Length < 4 || password.Length > 20 || string.IsNullOrEmpty(username) || username.Length > 12)
            {
                _logger.Information("Username ('{0}') or password did not pass validation.", username);
                return new LoginResponse<LoginStatus> { Status = LoginStatus.StatusInvalidCredentials };
            }

            var playerInDatabase = await _playerSerializer.GetAsync(username);

            if (playerInDatabase != null && !playerInDatabase.Password.Equals(password))
            {
                return new LoginResponse<LoginStatus> { Status = LoginStatus.StatusInvalidCredentials };
            }
            
            var createdNewPlayer = playerInDatabase == null;
            var player = await _playerSerializer.GetOrCreateAsync(request.Credentials);
            
            return new LoginResponse<LoginStatus> { Status = LoginStatus.StatusOk };
        }
    }
}
