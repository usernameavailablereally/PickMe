using Services;
using Services.Loaders.Configs;
using Services.Match;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Lifetimes
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private PickObjectManager _pickObjectManager;
        
        protected override void Configure(IContainerBuilder builder)
        {            
            ValidateSerializedFields();
            
            RegisterSceneComponents(builder);
            
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
        }
        
        private void RegisterInstances(IContainerBuilder builder)
        {
            builder.Register<AssetsLoader>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<MatchService>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    
        private void RegisterEntryPoints(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<EntryPointService>().AsSelf();
        }
    }
}
