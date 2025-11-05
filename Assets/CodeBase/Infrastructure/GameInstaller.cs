using System;
using CodeBase.Infrastructure.ECS;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.FSM;
using CodeBase.Infrastructure.Services;
using CodeBase.Infrastructure.Services.Asset;
using CodeBase.Infrastructure.Services.Config;
using CodeBase.Infrastructure.Services.Input;
using MessagePipe;
using Unity.Entities;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace CodeBase.Infrastructure
{
    public class GameInstaller : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            RegisterEcsWorld(builder);
            RegisterSystemEngine(builder);
            RegisterInput(builder);
            RegisterAssetProvider(builder);
            RegisterConfigProvider(builder);
            RegisterGameFactory(builder);
            RegisterStateMachine(builder);
            RegisterEntryPoint(builder);
            RegisterMessagePipe(builder);
            DontDestroyOnLoad(gameObject);
        }

        private static void RegisterMessagePipe(IContainerBuilder builder)
        {
            MessagePipeOptions options = builder.RegisterMessagePipe();
        }

        private void RegisterStateMachine(IContainerBuilder builder) =>
            builder.Register<IStateMachine, StateMachine>(Lifetime.Singleton);

        private void RegisterAssetProvider(IContainerBuilder builder) =>
            builder.Register<IAssetProvider, AssetProvider>(Lifetime.Singleton);

        private void RegisterGameFactory(IContainerBuilder builder) =>
            builder.Register<IGameFactory, GameFactory>(Lifetime.Singleton);

        private void RegisterConfigProvider(IContainerBuilder builder) =>
            builder.Register<IConfigProvider, ConfigProvider>(Lifetime.Singleton);

        private void RegisterInput(IContainerBuilder builder) =>
            builder.Register<IInputService, InputService>(Lifetime.Singleton);

        private void RegisterSystemEngine(IContainerBuilder builder) =>
            builder.Register<SystemEngine>(Lifetime.Singleton);

        private void RegisterEntryPoint(IContainerBuilder builder) =>
            builder.RegisterEntryPoint<EntryPoint>()
                .As<ITickable>()
                .As<IFixedTickable>()
                .As<ILateTickable>()
                .As<IPostTickable>()
                .As<IDisposable>();

        private void RegisterEcsWorld(IContainerBuilder builder)
        {
            var world = new World("GameWorld");

            builder.RegisterInstance(world)
                .As<World>()
                .AsSelf();
        }
    }
}