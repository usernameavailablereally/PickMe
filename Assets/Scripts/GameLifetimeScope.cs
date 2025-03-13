using Controllers;
using MonoBehaviours;
using Services;
using Services.Events;
using Services.Loaders.Configs;
using Services.Match;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private PickObjectManager _pickObjectManager;
    [SerializeField] private Camera _mainCamera;
        
    protected override void Configure(IContainerBuilder builder)
    {            
        ValidateSerializedFields();
            
        RegisterSceneComponents(builder);
            
        RegisterControllers(builder);
            
        RegisterInstances(builder);
            
        RegisterEntryPoints(builder);
    }

    private void ValidateSerializedFields()
    {
        if (_pickObjectManager == null) throw new MissingReferenceException($"{nameof(_pickObjectManager)} is not assigned");
    }    
        
    private void RegisterSceneComponents(IContainerBuilder builder)
    {
        builder.RegisterComponent(_pickObjectManager);
        builder.RegisterComponent(_mainCamera);
    }

    private void RegisterControllers(IContainerBuilder builder)
    {
        builder.Register<GameInputController>(Lifetime.Singleton).As<ITickable>();
    }
        
    private void RegisterInstances(IContainerBuilder builder)
    {
        builder.Register<AssetsLoader>(Lifetime.Singleton).AsImplementedInterfaces();
        builder.Register<MatchService>(Lifetime.Singleton).AsImplementedInterfaces();
        builder.Register<DispatcherService>(Lifetime.Singleton).AsImplementedInterfaces();
    }
    
    private void RegisterEntryPoints(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<EntryPointService>().AsSelf();
    }
}