using System.Threading;
using Cysharp.Threading.Tasks;
using MonoBehaviours;
using Services.Factories;
using Services.Loaders.Configs;

namespace Services.Match
{
    public class MatchService : IMatchService
    {
        // // public event Action<Team> GameEnded;
        // //
        // // private readonly IMatchConfigProvider _matchConfigProvider;
        // // private readonly IMatchUnitsService _matchUnitsService;
        // // private readonly IUnitSpawner _unitSpawner;
        //
        // private IUnitController _previousUnit;
        //
        // public MatchService(IMatchConfigProvider matchConfigProvider, IMatchUnitsService matchUnitsService,
        //     IUnitSpawner unitSpawner)
        // {
        //     _matchConfigProvider = matchConfigProvider;
        //     _unitSpawner = unitSpawner;
        //     _matchUnitsService = matchUnitsService;
        // }
        //
        // public async UniTask SetupUnits()
        // {
        //     var playerTask = SpawnUnits(_matchConfigProvider.GetPlayerUnits,
        //         _unitSpawner.SpawnPlayerUnit);
        //
        //     var enemyTask = SpawnUnits(_matchConfigProvider.GetEnemyUnits,
        //         _unitSpawner.SpawnEnemyUnit);
        //
        //     await UniTask.WhenAll(playerTask, enemyTask);
        //
        //     return;
        //
        //     async UniTask SpawnUnits<T>(Func<UniTask<IReadOnlyList<UnitConfig>>> getter,
        //         Func<UnitConfig, UniTask<T>> spawner) where T: IUnitController
        //     {
        //         var unitConfigs = await getter.Invoke();
        //         var tasks = new UniTask[unitConfigs.Count];
        //         for (int i = 0; i < unitConfigs.Count; i++)
        //         {
        //             tasks[i] = Spawn(unitConfigs[i]);
        //         }
        //
        //         await UniTask.WhenAll(tasks);
        //
        //         return;
        //
        //         async UniTask Spawn(UnitConfig unitConfig)
        //         {
        //             var unit = await spawner.Invoke(unitConfig);
        //             _matchUnitsService.Add(unit);
        //             unit.PerformedAttack += OnUnitAttack;
        //         }
        //     }
        // }
        //
        // public void StartGame() => NextTurn();
        //
        // private void OnUnitAttack(IUnitController attacker, IUnitController target)
        // {
        //     target.TakeDamage(attacker.Config.AttackPower);
        //     if (!target.IsAlive)
        //     {
        //         _matchUnitsService.Remove(target);
        //         target.PerformedAttack += OnUnitAttack;
        //         target.Kill();
        //         target.Dispose();
        //     }
        //
        //     if (CheckEndConditions())
        //     {
        //         return;
        //     }
        //
        //     _matchUnitsService.MoveNext();
        //     NextTurn();
        // }
        //
        // private void NextTurn()
        // {
        //     _previousUnit?.EndTurn();
        //     var next = _matchUnitsService.Next;
        //     next.StartTurn();
        //     _previousUnit = next;
        // }
        //
        // private bool CheckEndConditions()
        // {
        //     if (_matchUnitsService.Count(Team.Player) == 0)
        //     {
        //         GameEnded?.Invoke(Team.Enemy);
        //         return true;
        //     }
        //
        //     if (_matchUnitsService.Count(Team.Enemy) == 0)
        //     {
        //         GameEnded?.Invoke(Team.Player);
        //         return true;
        //     }
        //
        //     return false;
        // }

        private readonly IAssetsLoader _assetsLoader;
        private IItemsFactory _itemsFactory;
        MatchConfig _matchConfig;
        private PickObjectManager _pickObjectManager;
        private ItemController[] _roundItems;

        public MatchService(IAssetsLoader assetsLoader, PickObjectManager pickObjectManager)
        {
            _assetsLoader = assetsLoader;
            _pickObjectManager = pickObjectManager;
        }

        public async UniTask BuildScene(CancellationToken cancellationToken)
        {
            _matchConfig = await _assetsLoader.LoadMatchConfig();
            //IList<ItemController> itemsPrefabs = await _assetsLoader.LoadGamePrefabs();

            _itemsFactory = new ItemsFactory();
            await _itemsFactory.Init(_matchConfig.ItemPrefabs, _matchConfig.ItemsPerRoundCount, _matchConfig.Colors);
        }

        void GenerateRounds()
        {
        }

        void RunFirstRound()
        {
        }

        void ValidateLoadedResources()
        {
        }

        public UniTask RunGame(CancellationToken cancellationToken)
        {
            _roundItems = _itemsFactory.GetPortion(_matchConfig.ItemsPerRoundCount);
            _pickObjectManager.PlaceItems(_roundItems);
            return UniTask.CompletedTask;
        }

        public UniTask ClearScene()
        {
            _itemsFactory.Clear();
            return UniTask.CompletedTask;
        }
    }
}