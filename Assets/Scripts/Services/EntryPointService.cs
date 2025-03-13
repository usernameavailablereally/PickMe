using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Services.Loaders.Configs;
using Services.Match;
using VContainer;
using VContainer.Unity;

namespace Services
{
    // Generates initial grid. Playing life generations is in LifePlayer.cs 
    public class EntryPointService : IAsyncStartable, IDisposable
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
            _startCancellationTokenSource.Cancel();

            await _matchService.ClearScene();
 
            // New global game cancellation token
            _startCancellationTokenSource = new CancellationTokenSource();
            await StartAsync(_startCancellationTokenSource.Token);
        }
        
        public void Dispose()
        {
            _startCancellationTokenSource?.Dispose();
        }
    }
}