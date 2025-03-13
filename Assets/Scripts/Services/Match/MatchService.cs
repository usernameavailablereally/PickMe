using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MonoBehaviourComponents;
using Services.Events;
using Services.Factories;
using Services.Loaders.Configs;
using UnityEngine;

namespace Services.Match
{
    public class MatchService : IMatchService, IDisposable
    {
        private readonly IAssetsLoader _assetsLoader;
        private readonly IItemsFactory _itemsFactory;
        private readonly PickObjectComponent _pickObjectComponent;
        private readonly IDispatcherService _dispatcherService;
        private readonly RoundLogic _roundLogic;
        private CancellationTokenSource _roundTokenSource;
        private ItemComponent[] _roundItems;
        private MatchConfig _matchConfig;
        private int RoundsCounter { get; set; }
        
        public MatchService(IAssetsLoader assetsLoader, PickObjectComponent pickObjectComponent, 
            IDispatcherService dispatcherService)
        {
            _assetsLoader = assetsLoader;
            _pickObjectComponent = pickObjectComponent;
            _dispatcherService = dispatcherService;
            _roundLogic = new RoundLogic(dispatcherService);
            _itemsFactory = new ItemsFactory();
        }

        public async UniTask BuildScene(CancellationToken buildCancellationToken)
        {
            _dispatcherService.Subscribe<TargetItemClickedEvent>(OnTargetItemClicked);
            _matchConfig = await _assetsLoader.LoadAndValidateMatchConfig(buildCancellationToken); 
            await _itemsFactory.Init(_matchConfig, buildCancellationToken);
        } 

        public async UniTask RunGame()
        {
            try
            {
                RoundsCounter = 0;
                await StartRound();
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

            RoundsCounter++;
            if (IsGameFinished())
            {
                _dispatcherService.Dispatch(new RestartGameEvent());
                return UniTask.CompletedTask;
            }
            
            _dispatcherService.Dispatch(new RoundCounterUpdatedEvent(RoundsCounter, _matchConfig.RoundsCount));
            _roundItems = _itemsFactory.GetPortion(_matchConfig.ItemsPerRoundCount);
            _pickObjectComponent.PlaceItems(_roundItems);
            _roundLogic.StartRound(_roundItems, _roundTokenSource.Token);
            return UniTask.CompletedTask;
        }

        private bool IsGameFinished()
        {
            return RoundsCounter == _matchConfig.RoundsCount;
        }

        private void EndRound()
        {
            _roundLogic.EndRound();
            _itemsFactory.ReturnPortion(_roundItems);
        }

        private async void OnTargetItemClicked(TargetItemClickedEvent obj)
        {
            try
            {
                EndRound();
                await StartRound();
            }
            catch (Exception e)
            {
               Debug.LogError(e);
            }
        }

        public void Dispose()
        {
            _dispatcherService.Unsubscribe<TargetItemClickedEvent>(OnTargetItemClicked);
            _itemsFactory.Clear();
        }
    }
}