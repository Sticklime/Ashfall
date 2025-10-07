using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CodeBase.Infrastructure.Services.Asset
{
    public class AssetProvider : IAssetProvider
    {
        private readonly Dictionary<string, AsyncOperationHandle> _cache = new();
        private readonly Dictionary<string, List<AsyncOperationHandle>> _usedResources = new();

        public async UniTask<T> LoadAsync<T>(string address) where T : class
        {
            if (_cache.TryGetValue(address, out var completedHandle))
                return (T)completedHandle.Result;

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(address);
            RegisterCacheAndCleanup(address, handle);

            return await handle.Task;
        }

        public async UniTask<T> LoadAsync<T>(AssetReference assetReference) where T : class
        {
            if (_cache.TryGetValue(assetReference.AssetGUID, out var completedHandle))
                return (T)completedHandle.Result;

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(assetReference);
            RegisterCacheAndCleanup(assetReference.AssetGUID, handle);

            return await handle.Task;
        }

        public void Cleanup()
        {
            foreach (var handle in _usedResources.Values.SelectMany(resourceHandles => resourceHandles))
                Addressables.Release(handle);

            _cache.Clear();
            _usedResources.Clear();
        }

        private void RegisterCacheAndCleanup<T>(string key, AsyncOperationHandle<T> handle)
        {
            handle.Completed += completeHandle => _cache[key] = completeHandle;

            RegisterForCleanup(key, handle);
        }

        private void RegisterForCleanup<T>(string guid, AsyncOperationHandle<T> handle)
        {
            if (!_usedResources.TryGetValue(guid, out var resourceHandles))
            {
                resourceHandles = new List<AsyncOperationHandle>();
                _usedResources[guid] = resourceHandles;
            }

            resourceHandles.Add(handle);
        }
    }

    public interface IAssetProvider
    {
        UniTask<T> LoadAsync<T>(string address) where T : class;
        UniTask<T> LoadAsync<T>(AssetReference assetReference) where T : class;
        void Cleanup();
    }
}