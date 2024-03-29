﻿using Autofac;
using NetScape.Abstractions.FileSystem;
using NetScape.Abstractions.Interfaces.Login;
using NetScape.Abstractions.Model.Login;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetScape.Abstractions.Login
{
    public abstract class DefaultLoginProcessor<TRequest, TResponse> : ILoginProcessor<TRequest, TResponse>, 
        IStartable, 
        IDisposable
        where TRequest : LoginRequest<TResponse>
    {
        protected internal readonly ILogger _logger;
        private readonly IList<TRequest> _loginRequests = new List<TRequest>();

        private readonly object _lockObject = new object();

        private CancellationToken _cancellationToken;
        private CancellationTokenSource _cancellationTokenSource;

        public DefaultLoginProcessor(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Enqueues the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <exception cref="InvalidOperationException">Login already exists</exception>
        public void Enqueue(TRequest request)
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
        /// Processes a single by retriving the player from <see cref="IPlayerRepository"/>
        /// </summary>
        /// <param name="request">The login request.</param>
        /// <returns></returns>
        protected abstract Task<TResponse> ProcessAsync(TRequest request);       

        /// <summary>
        /// Handles the login queue
        /// </summary>
        private void ProcessLogins()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                lock (_lockObject)
                {
                    while (_loginRequests.Count > 0)
                    {
                        var requests = _loginRequests.Take(100);
                        var tasks = requests.Select(loginTask =>
                                (request: loginTask, responseTask: ProcessAsync(loginTask)))
                            .ToList();
                        Task.WhenAll(tasks.Select(t => t.responseTask)).GetAwaiter().GetResult();
                        tasks.ForEach(t =>
                        {
                            var responseTask = t.responseTask;
                            var request = t.request;
                            if (responseTask.IsCompletedSuccessfully)
                            {
                                _loginRequests.Remove(t.request);
                                t.request.Result = request.Result;
                                _ = request.OnResult(responseTask.Result).ConfigureAwait(false);
                                _logger.Debug("Processed Login Request: {@LoginRequest}", request.Credentials);
                            }
                            else
                            {
                                _logger.Error(responseTask.Exception, nameof(ProcessLogins));
                            }
                        });
                    }
                    Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// Perform once-off startup processing.
        /// </summary>
        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            Thread t = new Thread(ProcessLogins)
            {
                Name = "LoginProcessorThread"
            };
            t.Start();
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
