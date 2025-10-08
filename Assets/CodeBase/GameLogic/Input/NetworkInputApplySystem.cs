using CodeBase.GameLogic.Common;
using CodeBase.Infrastructure;
using Fusion;
using Scellecs.Morpeh;
using UnityEngine;

namespace CodeBase.GameLogic.Input
{
    public class NetworkInputApplySystem : ISystem
    {
        public World World { get; set; }

        private NetworkRunner _runner;

        public NetworkInputApplySystem(NetworkRunner runner)
        {
            _runner = runner;
        }

        public void OnAwake()
        {
        }

        public void OnUpdate(float deltaTime)
        {
            var filter = World.Filter
                .With<InputComponent>()
                .Build();

            foreach (var entity in filter)
            {
                ref var inputComponent = ref entity.GetComponent<InputComponent>();
                
                inputComponent.PlayerInput = inputComponent.NetworkInputReceiver.PlayerInput;
            }
        }

        public void Dispose()
        {
        }
    }
}