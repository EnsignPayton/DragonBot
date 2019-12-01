using System.Threading.Tasks;

namespace DragonBot.Utilities
{
    public interface IConfigProvider<T>
    {
        Task<T> GetConfigAsync();
        Task SaveConfigAsync(T config);
    }
}
