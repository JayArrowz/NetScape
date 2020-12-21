using Autofac;
using Microsoft.Extensions.Configuration;
using NetScape.Abstractions.FileSystem;
using NetScape.Abstractions.Interfaces.Login;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetScape.Modules.LoginProtocol
{
    public class LoginProcessor : ILoginProcessor<Rs2LoginRequest, Rs2LoginResponse>, IStartable, IDisposable
    {
        private ILogger _logger;
        private readonly IPlayerSerializer _playerSerializer;
        private readonly HashSet<Rs2LoginRequest> _loginRequests = new HashSet<Rs2LoginRequest>();
        private readonly HashSet<Rs2LoginRequest> _completedRequests = new HashSet<Rs2LoginRequest>();
        private readonly object _lockObject = new object();
        private readonly TimeSpan _loginProcessorTimeout;

        private CancellationToken _cancellationToken;
        private CancellationTokenSource _cancellationTokenSource;

        public LoginProcessor(ILogger logger, IPlayerSerializer playerSerializer, IConfigurationRoot configurationRoot)
        {
            _loginProcessorTimeout = TimeSpan.FromMilliseconds(int.Parse(configurationRoot["LoginProcessorTimeout"]));
            _logger = logger;
            _playerSerializer = playerSerializer;
        }

        public void Enqueue(Rs2LoginRequest request)
        {
            lock (_lockObject)
            {
                var loginExists = _loginRequests.Any(t => t.Credentials.Username.Equals(request.Credentials.Username, StringComparison.InvariantCultureIgnoreCase)
                && t.Credentials.Password.Equals(request.Credentials.Password, StringComparison.InvariantCultureIgnoreCase));

                if (!loginExists)
                {
                    _loginRequests.Add(request);
                }
            }
        }

        public async Task<Rs2LoginResponse> GetResultAsync(Rs2LoginRequest request)
        {
            using (var cancellationTokenSource =
                new CancellationTokenSource(_loginProcessorTimeout))
            {
                var cancellationToken = cancellationTokenSource.Token;
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (!_completedRequests.Contains(request))
                    {
                        //Wait 100MS for result
                        await Task.Delay(100);
                        continue;
                    } else
                    {
                        _completedRequests.Remove(request);
                        break;
                    }
                }

                cancellationToken.ThrowIfCancellationRequested();
            }
            return request.Result;
        }

        private async Task<Rs2LoginResponse> Process(Rs2LoginRequest request)
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

            return new Rs2LoginResponse { Status = LoginStatus.StatusOk };
        }

        private async Task ProcessLoginsAsync()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                while (_loginRequests.Count > 0)
                {
                    var requests = _loginRequests.ToList();
                    foreach (var loginRequest in requests)
                    {
                        var loginResult = await Process(loginRequest);
                        _loginRequests.Remove(loginRequest);
                        loginRequest.Result = loginResult;
                        _logger.Debug("Processed Login Request: {@LoginRequest}", loginRequest);
                        _completedRequests.Add(loginRequest);
                    }
                }
                await Task.Delay(600);
            }
        }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            Task.Factory.StartNew(ProcessLoginsAsync, _cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}
