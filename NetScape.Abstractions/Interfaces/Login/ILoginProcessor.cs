using NetScape.Abstractions.Login.Model;

namespace NetScape.Abstractions.Interfaces.Login
{
    public interface ILoginProcessor<TResponse>
    {
        LoginResponse<TResponse> Process(LoginRequest request);
    }
}
