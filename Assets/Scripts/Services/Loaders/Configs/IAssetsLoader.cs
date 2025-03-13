using System.Threading;
using Cysharp.Threading.Tasks;

namespace Services.Loaders.Configs
{
    // TODO split into separate interfaces
    public interface IAssetsLoader
    {
        UniTask<MatchConfig> LoadAndValidateMatchConfig(CancellationToken cancellationToken);
    }
}