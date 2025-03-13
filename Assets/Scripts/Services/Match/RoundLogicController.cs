using System.Threading;
using MonoBehaviours;
using Services.Events;
using UnityEngine;

namespace Services.Match
{
    public class RoundLogicController 
    {
        private readonly IDispatcherService _dispatcherService;
        private Color _targetColor;
        private CancellationToken _roundCancellationToken;
        public RoundLogicController(IDispatcherService dispatcherService)
        {
            _dispatcherService = dispatcherService;
        }
        
        public void StartRound(ItemController[] currentItems, CancellationToken roundCancellationToken)
        {
            _targetColor = ChooseRandomColor(currentItems);
            _roundCancellationToken = roundCancellationToken;
            _dispatcherService.Subscribe<ItemClickedEvent>(OnItemClicked);
        }

        private void OnItemClicked(ItemClickedEvent item)
        {
             if (item.Controller.Color == _targetColor)
             {
                 _dispatcherService.Dispatch(new TargetItemClickedEvent(item.Controller));
             }
             else
             {
                  item.Controller.AnimateWrong(_roundCancellationToken).Forget();
                 _dispatcherService.Dispatch(new WrongItemClickedEvent(item.Controller));
             }
        }

        public void EndRound()
        {
            _dispatcherService.Unsubscribe<ItemClickedEvent>(OnItemClicked);
        }
        
        private Color ChooseRandomColor(ItemController[] currentItems)
        {
            return currentItems[Random.Range(0, currentItems.Length)].Color;
        }
    }
}