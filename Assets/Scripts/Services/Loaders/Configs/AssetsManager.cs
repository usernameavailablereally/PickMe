using System.Threading;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Services.Loaders.Configs
{
    public class AssetsManager : IAssetsLoader
    { 
        public async UniTask<MatchConfig> LoadAndValidateMatchConfig(CancellationToken cancellationToken)
        {
            var matchConfig = await LoadAssetAsync<MatchConfig>(StringConstants.MATCH_CONFIG_ADDRESS, cancellationToken);
            ValidateMatchConfigAsserts(matchConfig);
            return matchConfig;
        }
        
        private static UniTask<T> LoadAssetAsync<T>(string address, CancellationToken cancellationToken)
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(address);
            return handle.ToUniTask(cancellationToken: cancellationToken);
        }

        private void ValidateMatchConfigAsserts(MatchConfig config)
        {
            Assert.IsNotNull(config, "MatchConfig is null");
            Assert.IsTrue(config.ItemPrefabs.Length > 0, "ItemPrefabs array is empty");
            Assert.IsTrue(config.ItemsPerRoundCount > 0, "ItemsPerRoundCount must be positive");
            Assert.IsTrue(config.RoundsCount > 0, "RoundsCount must be positive"); 
            Assert.IsTrue(config.Colors.Length > 0, "Colors array is empty");
            Assert.IsTrue(config.Colors.Length >= config.ItemsPerRoundCount, "Colors array must be at least as long as ItemsPerRoundCount");
        }
    }
}