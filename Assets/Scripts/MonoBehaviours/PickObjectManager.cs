using System.Collections.Generic;
using MonoBehaviours;
using UnityEngine;

public class PickObjectManager : MonoBehaviour
{
    [SerializeField] private Transform _itemsParent;
    [SerializeField] private Camera _mainCamera;

    public void PlaceItems(IList<ItemController> items)
    {
        if (items == null || items.Count == 0) return;

        float spacing = 1.0f / (items.Count + 1);

        for (var i = 0; i < items.Count; i++)
        {
            float viewportX = spacing * (i + 1);
            var viewportPosition = new Vector3(viewportX, 0.5f, _mainCamera.nearClipPlane + 2);
            Vector3 worldPosition = _mainCamera.ViewportToWorldPoint(viewportPosition);
            worldPosition.y = _itemsParent.position.y;
            items[i].transform.position = worldPosition;
            items[i].transform.SetParent(_itemsParent);
        }
    }
}