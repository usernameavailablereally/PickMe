using Cysharp.Threading.Tasks;
using MonoBehaviours;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Services.Factories
{
    public interface IItemsFactory
    {
        UniTask Init(AssetReference[] itemPrefabs, int duplicationCount, Color[] itemColors);
        ItemController[] GetPortion(int itemsCount);
        void ReturnPortion(ItemController[] items);
        void Clear();
    }
}