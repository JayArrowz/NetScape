using NetScape.Abstractions.FileSystem;
using NetScape.Abstractions.Login;
using Serilog;
using System.Threading.Tasks;

namespace NetScape.Modules.ThreeOneSeven.LoginProtocol
{

    public class LoginProcessor : DefaultLoginProcessor<Rs2LoginRequest, Rs2LoginResponse>
    {
        private readonly IPlayerSerializer _playerSerializer;

        public LoginProcessor(ILogger logger, IPlayerSerializer playerSerializer) : base(logger)
        {
            _playerSerializer = playerSerializer;
        }

        /// <summary>
        /// Processes a single by retriving the player from <see cref="IPlayerSerializer"/>
        /// </summary>
        /// <param name="request">The login request.</param>
        /// <returns></returns>
        protected override async Task<Rs2LoginResponse> ProcessAsync(Rs2LoginRequest request)
        {
            var password = request.Credentials.Password;
            var username = request.Credentials.Username;
            _logger.Information("Pending login from {0}", username);

            if (password.Length < 4 || password.Length > 20 || string.IsNullOrEmpty(username) || username.Length > 12)
            {
                _logger.Information("Username ('{0}') or password did not pass validation.", username);
                return new Rs2LoginResponse { Status = ThreeOneSevenLoginStatus.StatusInvalidCredentials };
            }

            var playerInDatabase = await _playerSerializer.GetAsync(username);

            if (playerInDatabase != null && !playerInDatabase.Password.Equals(password))
            {
                return new Rs2LoginResponse { Status = ThreeOneSevenLoginStatus.StatusInvalidCredentials };
            }

            var createdNewPlayer = playerInDatabase == null;

            var player = await _playerSerializer.GetOrCreateAsync(request.Credentials);
            return new Rs2LoginResponse { Status = ThreeOneSevenLoginStatus.StatusOk, Player = player, Created = createdNewPlayer };
        }
    }
}
