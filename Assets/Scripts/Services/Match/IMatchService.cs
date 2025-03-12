using System.Threading;
using Cysharp.Threading.Tasks;

namespace Services.Match
{
    public interface IMatchService
    {
        UniTask BuildScene(CancellationToken cancellationToken);
        UniTask RunGame(CancellationToken cancellationToken);
        UniTask ClearScene();
    }
}