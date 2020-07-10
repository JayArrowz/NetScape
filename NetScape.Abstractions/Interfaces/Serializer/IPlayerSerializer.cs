using System.Threading.Tasks;

namespace NetScape.Abstractions.Interfaces.Serializer
{
    public interface IPlayerSerializer<TOut, TIn>
    {
        Task<TOut> SerializeAsync(TIn param);

        Task<TIn> DeserializeAsync(TOut param);
    }
}
