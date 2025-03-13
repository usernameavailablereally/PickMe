using System.Threading;
using MonoBehaviourComponents;
using Services.Events;
using Services.Loaders.Configs;
using UnityEngine;

namespace Services.Match
{
    public class RoundLogic 
    {
        private readonly IDispatcherService _dispatcherService;
        private ColorDefinition _targetColorDefinition;
        private CancellationToken _roundCancellationToken;
        public RoundLogic(IDispatcherService dispatcherService)
        {
            _dispatcherService = dispatcherService;
        }
        
        public void StartRound(ItemComponent[] currentItems, CancellationToken roundCancellationToken)
        {
            _targetColorDefinition = ChooseRandomColor(currentItems);
            _dispatcherService.Dispatch(new TaskUpdatedEvent(_targetColorDefinition));
            _roundCancellationToken = roundCancellationToken;
            _dispatcherService.Subscribe<UnknownItemClickedEvent>(OnItemClicked);
        }

        private void OnItemClicked(UnknownItemClickedEvent unknownItem)
        {
             if (unknownItem.Component.Color == _targetColorDefinition.Color)
             {
                 Debug.Log($"Correct item picked (color: {unknownItem.Component.GetMaterialAlias().ColorDefinition.Name})");
                 _dispatcherService.Dispatch(new TargetItemClickedEvent(unknownItem.Component));
             }
             else
             {
                  unknownItem.Component.AnimateWrong(_roundCancellationToken).Forget();
                 Debug.Log($"Wrong item picked (color: {unknownItem.Component.GetMaterialAlias().ColorDefinition.Name}), target color: {_targetColorDefinition.Name}");
             }
        }

        public void EndRound()
        {
            _dispatcherService.Unsubscribe<UnknownItemClickedEvent>(OnItemClicked);
        }
        
        private ColorDefinition ChooseRandomColor(ItemComponent[] currentItems)
        {
            return currentItems[Random.Range(0, currentItems.Length)].GetMaterialAlias().ColorDefinition;
        }
    }
}