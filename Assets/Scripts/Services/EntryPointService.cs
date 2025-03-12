using System.Threading;
using Cysharp.Threading.Tasks;
using Services.Loaders.Configs;
using Services.Match;
using VContainer;
using VContainer.Unity;

namespace Services
{
    // Generates initial grid. Playing life generations is in LifePlayer.cs 
    public class EntryPointService : IAsyncStartable//, IRestartClickHandler
    {
        private readonly IMatchService _matchService;
        private AssetsLoader _loader;
        CancellationTokenSource _startCancellationTokenSource;
        
        [Inject]
        public EntryPointService(IMatchService matchService)
        {
            _matchService = matchService;
        } 

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            _startCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
            // here could be loading control, if there are many resources  
            
            // here could be Scene managing, if there are many
            await _matchService.BuildScene(cancellation);
            await _matchService.RunGame(cancellation);
        }  

        public void OnRestartClicked()
        {
            ReStartAsync().Forget();
        }

        private async UniTask ReStartAsync()
        {
            _startCancellationTokenSource.Cancel(); // stops current game if it is still running (or stucked)
            await _matchService.ClearScene();
            await StartAsync(CancellationToken.None);
            // Clear scene
            // StartAsync
        }  
    }
}