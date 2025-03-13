using System.Collections.Generic;
using System.Threading;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using MonoBehaviours;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Services.Loaders.Configs
{
    public class AssetsLoader : IAssetsLoader
    { 
        public UniTask<MatchConfig> LoadMatchConfig(CancellationToken cancellationToken)
        {
            return LoadAssetAsync<MatchConfig>(StringConstants.MATCH_CONFIG_ADDRESS, cancellationToken);
        }

        public async UniTask<IList<ItemController>> LoadGamePrefabs(CancellationToken cancellationToken)
        {
            return await LoadAssetsAsync<ItemController>(StringConstants.MATCH_PREFABS_BUNDLE, cancellationToken);
        }
        private static UniTask<T> LoadAssetAsync<T>(string address, CancellationToken cancellationToken)
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(address);
            return handle.ToUniTask(cancellationToken: cancellationToken);
        }

        private static async UniTask<IList<T>> LoadAssetsAsync<T>(string address, CancellationToken cancellationToken)
        {
            AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(address);
            return await handle.ToUniTask(cancellationToken: cancellationToken);
        }
    }
}