using System.Threading.Tasks;

namespace SimpleFinanceTestTask.ApplicationCore.Interfaces.Services
{
    public interface IPoolObjectAppService
    {
        Task<(bool response, object item)> TryGetItemAsync(int itemId);
        Task<bool> TryReleaseItemAsync(int itemId);
    }
}
