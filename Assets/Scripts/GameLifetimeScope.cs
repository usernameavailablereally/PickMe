using Controllers;
using MonoBehaviourComponents;
using Services;
using Services.Events;
using Services.Loaders.Configs;
using Services.Match;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private PickObjectComponent _pickObjectComponent;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private RoundsCounterComponent _roundsCounterComponent;
    [SerializeField] private TaskViewerComponent _taskViewerComponent;
        
    protected override void Configure(IContainerBuilder builder)
    {            
        ValidateSerializedFields();
            
        RegisterMonoComponents(builder);
            
        RegisterControllers(builder);
            
        RegisterInstances(builder);
            
        RegisterEntryPoints(builder);
    }

    private void ValidateSerializedFields()
    {
        if (_pickObjectComponent == null) throw new MissingReferenceException($"{nameof(_pickObjectComponent)} is not assigned");
        if (_mainCamera == null) throw new MissingReferenceException($"{nameof(_mainCamera)} is not assigned");
        if (_roundsCounterComponent == null) throw new MissingReferenceException($"{nameof(_roundsCounterComponent)} is not assigned");
        if (_taskViewerComponent == null) throw new MissingReferenceException($"{nameof(_taskViewerComponent)} is not assigned");
    }    
        
    private void RegisterMonoComponents(IContainerBuilder builder)
    {
        builder.RegisterComponent(_pickObjectComponent);
        builder.RegisterComponent(_mainCamera);
        builder.RegisterComponent(_roundsCounterComponent);
        builder.RegisterComponent(_taskViewerComponent);
    }

    private void RegisterControllers(IContainerBuilder builder)
    {
        builder.Register<GameInputController>(Lifetime.Singleton).As<ITickable>();
    }
        
    private void RegisterInstances(IContainerBuilder builder)
    {
        builder.Register<AssetsManager>(Lifetime.Singleton).AsImplementedInterfaces();
        builder.Register<MatchService>(Lifetime.Singleton).AsImplementedInterfaces();
        builder.Register<DispatcherService>(Lifetime.Singleton).AsImplementedInterfaces();
    }
    
    private void RegisterEntryPoints(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<EntryPointService>().AsSelf();
    }
}