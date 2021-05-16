using Autofac;
using NetScape.Abstractions.FileSystem;
using NetScape.Abstractions.Interfaces.Login;
using NetScape.Modules.FourSevenFour.LoginProtocol;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetScape.Modules.FourSevenFour.LoginProtocol
{
    public class LoginProcessor : ILoginProcessor<Rs2LoginRequest, Rs2LoginResponse>, IStartable, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IPlayerSerializer _playerSerializer;
        private readonly IList<Rs2LoginRequest> _loginRequests = new List<Rs2LoginRequest>();

        private readonly object _lockObject = new object();

        private CancellationToken _cancellationToken;
        private CancellationTokenSource _cancellationTokenSource;

        public LoginProcessor(ILogger logger, IPlayerSerializer playerSerializer)
        {
            _logger = logger;
            _playerSerializer = playerSerializer;
        }

        /// <summary>
        /// Enqueues the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <exception cref="InvalidOperationException">Login already exists</exception>
        public void Enqueue(Rs2LoginRequest request)
        {
            lock (_lockObject)
            {
                var loginExists = _loginRequests.Any(t => t.Credentials.Username.Equals(request.Credentials.Username, StringComparison.InvariantCultureIgnoreCase)
                && t.Credentials.Password.Equals(request.Credentials.Password, StringComparison.InvariantCultureIgnoreCase));

                if (loginExists)
                {
                    throw new InvalidOperationException("Login already exists");
                }

                _loginRequests.Add(request);
            }
        }

        /// <summary>
        /// Processes a single by retriving the player from <see cref="IPlayerSerializer"/>
        /// </summary>
        /// <param name="request">The login request.</param>
        /// <returns></returns>
        private async Task<Rs2LoginResponse> ProcessAsync(Rs2LoginRequest request)
        {
            var password = request.Credentials.Password;
            var username = request.Credentials.Username;
            _logger.Information("Pending login from {0}", username);

            if (password.Length < 4 || password.Length > 20 || string.IsNullOrEmpty(username) || username.Length > 12)
            {
                _logger.Information("Username ('{0}') or password did not pass validation.", username);
                return new Rs2LoginResponse { Status = LoginStatus.StatusInvalidCredentials };
            }

            var playerInDatabase = await _playerSerializer.GetAsync(username);

            if (playerInDatabase != null && !playerInDatabase.Password.Equals(password))
            {
                return new Rs2LoginResponse { Status = LoginStatus.StatusInvalidCredentials };
            }

            var createdNewPlayer = playerInDatabase == null;

            var player = await _playerSerializer.GetOrCreateAsync(request.Credentials);
            return new Rs2LoginResponse { Status = LoginStatus.StatusOk, Player = player, Created = createdNewPlayer };
        }

        /// <summary>
        /// Handles the login queue
        /// </summary>
        private async Task ProcessLoginsAsync()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                while (_loginRequests.Count > 0)
                {
                    var requests = _loginRequests.ToList();
                    var tasks = requests.Select(loginTask =>
                    (request: loginTask, responseTask: ProcessAsync(loginTask)))
                        .ToList();
                    await Task.WhenAll(tasks.Select(t => t.responseTask));
                    tasks.ForEach(t =>
                    {
                        var responseTask = t.responseTask;
                        var request = t.request;
                        if (responseTask.IsCompletedSuccessfully)
                        {
                            _loginRequests.Remove(t.request);
                            t.request.Result = request.Result;
                            _ = request.OnResult(responseTask.Result);
                            _logger.Debug("Processed Login Request: {@LoginRequest}", request.Credentials);
                        }
                    });
                }
                await Task.Delay(600);
            }
        }

        /// <summary>
        /// Perform once-off startup processing.
        /// </summary>
        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            Task.Factory.StartNew(ProcessLoginsAsync, _cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}
