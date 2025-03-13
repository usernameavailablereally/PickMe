using MonoBehaviourComponents;
using Services.Events;
using UnityEngine;
using VContainer.Unity;

namespace Controllers
{
    public class GameInputController : ITickable
    {
        private readonly IDispatcherService _dispatcherService; 
        private readonly Camera _mainCamera;
        private bool _isWaitingForMouseUp;    
        private bool IsLeftButtonDown() => Input.GetMouseButtonDown(0);
        private bool IsLeftButtonUp() => Input.GetMouseButtonUp(0);
        
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
            if (_isWaitingForMouseUp)
            {
                if (IsLeftButtonUp())
                {
                    _isWaitingForMouseUp = false;
                }
                return;
            }

            if (IsLeftButtonDown())
            {
                if (TryGetItemUnderCursor(out ItemComponent itemController))
                {
                    _dispatcherService.Dispatch(new UnknownItemClickedEvent(itemController));
                }
                _isWaitingForMouseUp = true;
            }
        }
        
        private bool TryGetItemUnderCursor(out ItemComponent itemComponent)
        {
            itemComponent = null;
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit)) return false;
            
            itemComponent = hit.collider.GetComponent<ItemComponent>();
            return itemComponent != null;
        }
    }
}