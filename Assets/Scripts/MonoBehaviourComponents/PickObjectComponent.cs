using System.Collections.Generic;
using UnityEngine;

namespace MonoBehaviourComponents
{
    public class PickObjectComponent : MonoBehaviour
    {
        [SerializeField] private Transform _itemsParent;
        [SerializeField] private Camera _mainCamera;
        
        private const float CAMERA_Z_OFFSET = 2;
        private const float VIEWPORT_Y_OFFSET = 0.5f;
        private const float SPACING_FACTOR = 1.0f;

        public void PlaceItems(IList<ItemComponent> items)
        {
            if (items == null || items.Count == 0) return;

            float spacing = SPACING_FACTOR / (items.Count + 1);

            for (var i = 0; i < items.Count; i++)
            {
                float viewportX = spacing * (i + 1);
                var viewportPosition = new Vector3(viewportX, VIEWPORT_Y_OFFSET, _mainCamera.nearClipPlane + CAMERA_Z_OFFSET);
                Vector3 worldPosition = _mainCamera.ViewportToWorldPoint(viewportPosition);
                worldPosition.y = _itemsParent.position.y; 
                items[i].transform.SetParent(_itemsParent);
                items[i].transform.position = worldPosition;
            }
        }
    }
}