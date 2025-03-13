using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MonoBehaviours;
using Services.Events;
using Services.Factories;
using Services.Loaders.Configs;

namespace Services.Match
{
    public class MatchService : IMatchService, IDisposable
    {
        private readonly IAssetsLoader _assetsLoader;
        private readonly IItemsFactory _itemsFactory;
        private readonly PickObjectManager _pickObjectManager;
        private readonly IDispatcherService _dispatcherService;
        private readonly RoundLogicController _roundLogicController;
        private CancellationTokenSource _gameTokenSource;
        private CancellationTokenSource _roundTokenSource;
        private ItemController[] _roundItems;
        private MatchConfig _matchConfig;
        public MatchService(IAssetsLoader assetsLoader, PickObjectManager pickObjectManager, 
            IDispatcherService dispatcherService)
        {
            _assetsLoader = assetsLoader;
            _pickObjectManager = pickObjectManager;
            _dispatcherService = dispatcherService;
            _roundLogicController = new RoundLogicController(dispatcherService);
            _itemsFactory = new ItemsFactory();
        }

        public async UniTask BuildScene(CancellationToken cancellationToken)
        {
            _dispatcherService.Subscribe<TargetItemClickedEvent>(OnTargetItemClicked);
            _matchConfig = await _assetsLoader.LoadMatchConfig(cancellationToken);
            await _itemsFactory.Init(_matchConfig, cancellationToken);
        }

        public UniTask ClearScene()
        {
            _dispatcherService.Unsubscribe<TargetItemClickedEvent>(OnTargetItemClicked);
            return UniTask.CompletedTask;
        }

        public async UniTask RunGame(CancellationToken cancellationToken)
        {
            try
            {
                _gameTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                await StartRound();

                // Global gameToken await in addition to roundToken.
                // Debatable for this case, but I'll keep it
                await UniTask.WaitUntilCanceled(_gameTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                Dispose();
            }
        }

        private UniTask StartRound()
        {
            // Round token linked to round-items animations
            _roundTokenSource?.Cancel();
            _roundTokenSource = new CancellationTokenSource();

            _roundItems = _itemsFactory.GetPortion(_matchConfig.ItemsPerRoundCount);
            _pickObjectManager.PlaceItems(_roundItems);
            _roundLogicController.StartRound(_roundItems, _roundTokenSource.Token);
            return UniTask.CompletedTask;
        }
        
        private void EndRound()
        {
            _roundLogicController.EndRound();
            _itemsFactory.ReturnPortion(_roundItems);
        }

        private void OnTargetItemClicked(TargetItemClickedEvent obj)
        {
            EndRound();
            StartRound();
        }

        public void Dispose()
        {
            _roundTokenSource?.Cancel();
            _roundTokenSource?.Dispose();
            _gameTokenSource?.Cancel();
            _gameTokenSource?.Dispose();
        }
    }
}