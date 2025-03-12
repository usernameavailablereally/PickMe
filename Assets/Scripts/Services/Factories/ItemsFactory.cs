using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MonoBehaviours;
using Services.Factories.Pools;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Services.Factories
{
    public class ItemsFactory : IItemsFactory
    {
        private ObjectPool<GameObject> _objectPool;
        private MaterialBank _materialBank;
        private IItemsFactory _itemsFactoryImplementation;

        public async UniTask Init(AssetReference[] itemPrefabs, int duplicationCount, Color[] itemColors)
        {
            await InitItemsPool(itemPrefabs, duplicationCount);
            await InitMaterialsBank(itemColors);
        } 
        
        /// <summary>
        /// Pool of objects to be reused in rounds.
        /// We assume that we need duplications of the same prefab in the pool.
        /// If there are 3 items in round, cubes or spheres, we need 3*cubes and 3*spheres in the pool.
        /// So we will be able to randomly get 3 cubes in a row, for example.
        /// No overflow support needed, since the ItemsPerRound and RoundCount are fixed.
        /// </summary>
        private async UniTask InitItemsPool(AssetReference[] itemPrefabs, int duplicationCount)
        {
            var instantiatedPrefabs = new List<GameObject>();

            foreach (AssetReference prefabReference in itemPrefabs)
            {
                for (int i = 0; i < duplicationCount; i++)
                {
                    AsyncOperationHandle<GameObject> handle = prefabReference.InstantiateAsync();
                    await handle.Task;
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        GameObject instantiatedObject = handle.Result;
                        instantiatedPrefabs.Add(instantiatedObject);
                    }
                    else
                    {
                        Debug.LogError($"Failed to instantiate prefab: {prefabReference.RuntimeKey}");
                    }
                }
            }

            _objectPool = new ObjectPool<GameObject>(instantiatedPrefabs);
        }

        private UniTask InitMaterialsBank(Color[] colors)
        {
            _materialBank = new MaterialBank(colors);
            return UniTask.CompletedTask;
        } 

        public ItemController[] GetPortion(int itemsCount)
        {
            var items = new ItemController[itemsCount];
            for (var i = 0; i < itemsCount; i++)
            {
                ItemController obj = Get();
                items[i] = obj;
            }

            return items;
        }

        public void ReturnPortion(ItemController[] items)
        {
            foreach (ItemController item in items)
            {
                Return(item);
            }
        }

        public void Clear()
        {
            
        }

        private ItemController Get()
        {
            GameObject obj = _objectPool.GetRandomNext();
            Material mat = _materialBank.Get();
            obj.GetComponent<Renderer>().material = mat;
            return obj.GetComponent<ItemController>();
        }
        
        private void Return(ItemController obj)
        {
            _objectPool.ReturnToPool(obj.gameObject);
            _materialBank.ReturnToPool(obj.GetComponent<Renderer>().material);
        }
    }
}