using NetScape.Abstractions.Login.Model;
using System.Threading;
using System.Threading.Tasks;

namespace NetScape.Abstractions.Interfaces.Login
{
    public interface ILoginProcessor<TRequest, TResponse> where TRequest : LoginRequest<TResponse>
    {
        void Enqueue(TRequest request);

        Task<TResponse> GetResultAsync(TRequest request);
    }
}
