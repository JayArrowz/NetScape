using NetScape.Abstractions.Model.Login;

namespace NetScape.Abstractions.Interfaces.Login
{
    /// <summary>
    /// The login processor is in-charge of processing logins in a async manner
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public interface ILoginProcessor<TRequest, TResponse> where TRequest : LoginRequest<TResponse>
    {
        /// <summary>
        /// Enqueues the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        void Enqueue(TRequest request);
    }
}
