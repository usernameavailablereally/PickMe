using MonoBehaviours;
using Services.Events;
using UnityEngine;
using VContainer.Unity;

namespace Controllers
{
    public class GameInputController : ITickable
    {
        private readonly IDispatcherService _dispatcherService; 
        private readonly Camera _mainCamera;
        private bool IsLeftButtonDown() => Input.GetMouseButtonDown(0);
        
        public GameInputController(IDispatcherService dispatcherService, Camera mainCamera)
        {
            _dispatcherService = dispatcherService;
            _mainCamera = mainCamera;
        }

        public void Tick()
        {
            CheckForMouseInput();
        }

        private void CheckForMouseInput()
        {
            if (IsLeftButtonDown())
            {
                if (TryGetItemUnderCursor(out ItemController itemController))
                {
                    _dispatcherService.Dispatch(new ItemClickedEvent(itemController));
                }
            }
        }
        
        private bool TryGetItemUnderCursor(out ItemController itemController)
        {
            itemController = null;
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit)) return false;
            
            itemController = hit.collider.GetComponent<ItemController>();
            return itemController != null;
        }
    }
}