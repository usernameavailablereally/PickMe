using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Services.Events;
using Services.Match;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Services
{
    public class EntryPointService : IAsyncStartable, IDisposable
    {
        private readonly IMatchService _matchService;
        private readonly IDispatcherService _dispatcherService;
        private CancellationTokenSource _gameStartCancellationTokenSource;
        private bool _isRestarting;
        private bool _disposed;

        [Inject]
        public EntryPointService(IMatchService matchService, IDispatcherService dispatcherService)
        {
            _matchService = matchService ?? throw new ArgumentNullException(nameof(matchService));
            _dispatcherService = dispatcherService ?? throw new ArgumentNullException(nameof(dispatcherService));
            _dispatcherService.Subscribe<RestartGameEvent>(OnRestartTriggered);
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(EntryPointService));
        
            _gameStartCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
        
            await _matchService.BuildScene(_gameStartCancellationTokenSource.Token);
            await _matchService.RunGame();
        }

        private void OnRestartTriggered(RestartGameEvent data)
        {
            if (_isRestarting || _disposed) return;
            Debug.Log("<color=white>Restarting game...</color>");
            ReStartAsync().Forget();
        }

        private async UniTask ReStartAsync()
        {
            try
            {
                _isRestarting = true;
                _gameStartCancellationTokenSource?.Cancel();
                _gameStartCancellationTokenSource?.Dispose();

                // no crucial need to dispose and load again assets, just showing how lifecycle is clean and idempotent 
                // can be easily removed if _matchService.BuildScene will be moved outside the ReStart pipeline
                _matchService.Dispose();

                _gameStartCancellationTokenSource = new CancellationTokenSource();
                await StartAsync(_gameStartCancellationTokenSource.Token);
            }
            catch (Exception)
            {
                Debug.Log("<color=red>Failed to restart game</color>");
                _isRestarting = false;  
                throw;
            }
            finally
            {
                _isRestarting = false;  
            } 
        }

        public void Dispose()
        {
            if (_disposed) return;
            
            _disposed = true;
            _isRestarting = false;
            _gameStartCancellationTokenSource?.Cancel();
            _gameStartCancellationTokenSource?.Dispose();
            _gameStartCancellationTokenSource = null;
            _dispatcherService.Unsubscribe<RestartGameEvent>(OnRestartTriggered);
            _dispatcherService.ClearAllSubscriptions();
        }
    }
}