using Services.Events;
using TMPro;
using UnityEngine;
using VContainer;

namespace MonoBehaviourComponents
{
    public class TaskViewerComponent : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _taskViewerText;
    
        private IDispatcherService _dispatcherService;
    
        [Inject]
        private void Construct(IDispatcherService dispatcherService)
        {
            _dispatcherService = dispatcherService;
        }

        private void Start()
        {
            _dispatcherService.Subscribe<TaskUpdatedEvent>(OnTaskUpdated);
        }
    
        private void OnDestroy()
        {
            _dispatcherService.Unsubscribe<TaskUpdatedEvent>(OnTaskUpdated);
        }

        private void OnTaskUpdated(TaskUpdatedEvent obj)
        {
            var colorHex = $"#{ColorUtility.ToHtmlStringRGB(obj.TargetColor.Color)}";
            _taskViewerText.text = $"Find : <color={colorHex}>{obj.TargetColor.Name}</color>";
            Debug.Log($"Task updated to {obj.TargetColor.Name}");
        }
    }
}
