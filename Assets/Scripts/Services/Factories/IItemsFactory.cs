using System.Threading;
using Cysharp.Threading.Tasks;
using MonoBehaviourComponents;
using Services.Loaders.Configs;

namespace Services.Factories
{
    public interface IItemsFactory
    {
        UniTask Init(MatchConfig matchConfig, CancellationToken cancellationToken);
        ItemComponent[] GetPortion(int itemsCount);
        void ReturnPortion(ItemComponent[] items);
        void Clear();
    }
}