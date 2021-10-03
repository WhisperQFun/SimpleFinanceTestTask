using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using SimpleFinanceTestTask.ApplicationCore.Interfaces.Services;

namespace SimpleFinanceTestTask.Services
{
    public class PoolObjectAppService : IPoolObjectAppService
    {
        private readonly int _timeout = 5 * 1000;
        private readonly int _size = 10;


        private ConcurrentDictionary<int, SemaphoreSlim> _locks = new ConcurrentDictionary<int, SemaphoreSlim>();
        private ConcurrentDictionary<int, object> _items = new ConcurrentDictionary<int, object>();
        public PoolObjectAppService()
        {
            for (int i = 1; i <= _size; i++)
                _items.TryAdd(i, new object());
        }

        public async Task<(bool response, object item)> TryGetItemAsync(int itemId)
        {
            var cancelToken = new CancellationToken();

            var hasItemsLock = _locks.TryGetValue(itemId, out var lockSemaphoreSlim);

            if (hasItemsLock is false)
            {
                var itemLock = _locks.GetOrAdd(itemId, k => new SemaphoreSlim(1, 1));

                var waitTryResponse = await itemLock.WaitAsync(_timeout, cancelToken);

                if (waitTryResponse is false)
                    return (false, null);

                var itemPool = _items.TryGetValue(itemId, out var itemValue);

                return itemPool is true ? (true, itemValue) : (false, null);
            }
            else
            {
                var waitTryResponse = await lockSemaphoreSlim.WaitAsync(_timeout, cancelToken);

                if (waitTryResponse is false)
                    return (false, null);

                var itemPool = _items.TryGetValue(itemId, out var itemValue);

                return itemPool is true ? (true, itemValue) : (false, null);
            }
        }

        public async Task<bool> TryReleaseItemAsync(int itemId)
        {
            var cancelToken = new CancellationToken();

            var hasItemsLock = _locks.TryGetValue(itemId, out var lockSemaphoreSlim);

            if (hasItemsLock is false)
                return false;

            lockSemaphoreSlim.Release();
            return true;
        }
    }
}
