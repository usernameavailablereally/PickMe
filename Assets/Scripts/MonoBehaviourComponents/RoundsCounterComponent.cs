using Services.Events;
using TMPro;
using UnityEngine;
using VContainer;

namespace MonoBehaviourComponents
{
    public class RoundsCounterComponent : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _roundsCounterText;
        private IDispatcherService _dispatcherService;
    
        [Inject]    
        public void Construct(IDispatcherService dispatcherService)
        {
            _dispatcherService = dispatcherService;
        } 
        private void Start()
        {
            _dispatcherService.Subscribe<RoundCounterUpdatedEvent>(OnRoundStarted);
        }
        
        private void OnRoundStarted(RoundCounterUpdatedEvent roundCounterUpdatedEvent)
        {
            Debug.Log($"<color=yellow>Round {roundCounterUpdatedEvent.CurrentRound} of {roundCounterUpdatedEvent.RoundsCount} started</color>");
            _roundsCounterText.text = $"{roundCounterUpdatedEvent.CurrentRound} / {roundCounterUpdatedEvent.RoundsCount}";
        }
        
        private void OnDestroy()
        {
            _dispatcherService.Unsubscribe<RoundCounterUpdatedEvent>(OnRoundStarted);
        }
    }
}
