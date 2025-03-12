using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using MonoBehaviours;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Services.Loaders.Configs
{
    public class AssetsLoader : IAssetsLoader
    { 
        public UniTask<MatchConfig> LoadMatchConfig()
        {
            return LoadAssetAsync<MatchConfig>(StringConstants.MatchConfigAddress);
        }

        public async UniTask<IList<ItemController>> LoadGamePrefabs()
        {
            return await LoadAssetsAsync<ItemController>(StringConstants.MatchPrefabsBundle);
        }
        private static UniTask<T> LoadAssetAsync<T>(string address) 
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(address);
            return handle.ToUniTask();
        }

        private static async UniTask<IList<T>> LoadAssetsAsync<T>(string address)
        {
            AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(address);
            return await handle.ToUniTask();
        }
    }
}