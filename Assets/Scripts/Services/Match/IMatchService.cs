using System.Threading;
using Cysharp.Threading.Tasks;

namespace Services.Match
{
    public interface IMatchService
    {
        UniTask BuildScene(CancellationToken buildCancellationToken);
        UniTask RunGame();
        void Dispose();
    }
}