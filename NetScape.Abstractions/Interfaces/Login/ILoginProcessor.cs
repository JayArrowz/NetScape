using NetScape.Abstractions.Login.Model;
using System.Threading.Tasks;

namespace NetScape.Abstractions.Interfaces.Login
{
    public interface ILoginProcessor<TResponse>
    {
        Task<LoginResponse<TResponse>> ProcessAsync(LoginRequest request);
    }
}
