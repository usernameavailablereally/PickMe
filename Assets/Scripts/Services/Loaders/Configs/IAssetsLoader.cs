using System.Threading;
using Cysharp.Threading.Tasks;

namespace Services.Loaders.Configs
{
    public interface IAssetsLoader
    {
        UniTask<MatchConfig> LoadAndValidateMatchConfig(CancellationToken cancellationToken);
    }
}