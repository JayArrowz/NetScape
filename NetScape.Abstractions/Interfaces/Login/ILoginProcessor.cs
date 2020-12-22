using NetScape.Abstractions.Model.Login;

namespace NetScape.Abstractions.Interfaces.Login
{
    public interface ILoginProcessor<TRequest, TResponse> where TRequest : LoginRequest<TResponse>
    {
        void Enqueue(TRequest request);
    }
}
