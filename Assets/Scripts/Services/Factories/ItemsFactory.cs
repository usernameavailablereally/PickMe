using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MonoBehaviours;
using Services.Factories.Pools;
using Services.Loaders.Configs;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Services.Factories
{
public class ItemsFactory : IItemsFactory
{
    private ObjectPool<ItemController> _objectPool;
    private MaterialBank _materialBank;
    private readonly List<AsyncOperationHandle<GameObject>> _handles = new();

    public async UniTask Init(MatchConfig matchConfig, CancellationToken cancellationToken)
    {
        try
        {
            List<ItemController> prefabs = await LoadPrefabs(matchConfig.ItemPrefabs, matchConfig.ItemsPerRoundCount, cancellationToken);
            _objectPool = new ObjectPool<ItemController>(prefabs);
            _materialBank = new MaterialBank(matchConfig.Colors);
        }
        catch
        {
            Clear();
            throw;
        }
    }
    /// <summary>
    /// LoadPrefabs objects to be reused in rounds.
    /// We assume that we need duplications of the same prefab in the pool.
    /// If there are 3 items in round, cubes or spheres, we need 3*cubes and 3*spheres in the pool.
    /// So we will be able to randomly get 3 cubes in a row, for example.
    /// No overflow support needed, since the ItemsPerRound and RoundCount are fixed.
    /// </summary>
    private async UniTask<List<ItemController>> LoadPrefabs(AssetReference[] itemPrefabs, int duplicationCount,
        CancellationToken cancellationToken)
    {
        var prefabs = new List<ItemController>();

        foreach (AssetReference prefab in itemPrefabs)
        {
            for (int i = 0; i < duplicationCount; i++)
            {
                AsyncOperationHandle<GameObject> handle = prefab.InstantiateAsync();
                _handles.Add(handle);
                
                GameObject instance = await handle.WithCancellation(cancellationToken);
                instance.SetActive(false);
                prefabs.Add(instance.GetComponent<ItemController>());
            }
        }

        return prefabs;
    }

    public ItemController[] GetPortion(int itemsCount)
    {
        return Enumerable.Range(0, itemsCount)
            .Select(_ => Get())
            .ToArray();
    }

    private ItemController Get()
    {
        ItemController item = _objectPool.GetRandomNext();
        Material material = _materialBank.Get();
        
        item.SetMaterial(material);
        item.Activate();
        return item;
    }

    public void ReturnPortion(ItemController[] items)
    {
        if (items == null) return;
        Array.ForEach(items, Return);
    }

    private void Return(ItemController item)
    {
        if (item == null) return;

        _materialBank.Return(item.GetMaterial());
        
        // I'm not prefer pools to be responsible for disabling objects
        item.Deactivate();
        _objectPool.ReturnToPool(item);
    }

    public void Clear()
    {
        foreach (AsyncOperationHandle<GameObject> handle in _handles)
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }
        _handles.Clear();

        _objectPool?.Clear();
        _materialBank?.Clear();

        _objectPool = null;
        _materialBank = null;
    }
}
}