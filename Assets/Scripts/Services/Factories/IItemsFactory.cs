using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MonoBehaviours;
using Services.Loaders.Configs;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Services.Factories
{
    public interface IItemsFactory
    {
        UniTask Init(MatchConfig matchConfig, CancellationToken cancellationToken);
        ItemController[] GetPortion(int itemsCount);
        void ReturnPortion(ItemController[] items);
        void Clear();
    }
}