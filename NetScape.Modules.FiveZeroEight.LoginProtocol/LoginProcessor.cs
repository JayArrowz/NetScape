using NetScape.Abstractions.FileSystem;
using NetScape.Abstractions.Login;
using Serilog;
using System.Threading.Tasks;

namespace NetScape.Modules.FiveZeroEight.LoginProtocol
{
    public class LoginProcessor : DefaultLoginProcessor<FiveZeroEightLoginRequest, FiveZeroEightLoginResponse>
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
        protected override async Task<FiveZeroEightLoginResponse> ProcessAsync(FiveZeroEightLoginRequest request)
        {
            var password = request.Credentials.Password;
            var username = request.Credentials.Username;
            _logger.Information("Pending login from {0}", username);

            if (password.Length < 4 || password.Length > 20 || string.IsNullOrEmpty(username) || username.Length > 12)
            {
                _logger.Information("Username ('{0}') or password did not pass validation.", username);
                return new FiveZeroEightLoginResponse { Status = FiveZeroEightLoginStatus.StatusInvalidCredentials };
            }

            var playerInDatabase = await _playerSerializer.GetAsync(username);

            if (playerInDatabase != null && !playerInDatabase.Password.Equals(password))
            {
                return new FiveZeroEightLoginResponse { Status = FiveZeroEightLoginStatus.StatusInvalidCredentials };
            }

            var createdNewPlayer = playerInDatabase == null;

            var player = await _playerSerializer.GetOrCreateAsync(request.Credentials);
            return new FiveZeroEightLoginResponse { Status = FiveZeroEightLoginStatus.StatusOk, Player = player, Created = createdNewPlayer };
        }
    }
}
