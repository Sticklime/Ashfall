using CodeBase.GameLogic.Common;
using Scellecs.Morpeh;
using UnityEngine;

namespace CodeBase.GameLogic.Input
{
    public class InputConnectorSystem : ISystem
    {
        public World World { get; set; }

        public void Dispose()
        {
        }

        public void OnUpdate(float deltaTime)
        {
            var inputOnClient = World.Filter.With<InputNetworkComponent>().Build();
            var inputOnPlayer = World.Filter.With<InputComponent>().Build();

            foreach (var inputClient in inputOnClient)
            {
                ref var inputClientComponent = ref inputClient.GetComponent<InputNetworkComponent>();
                
                foreach (var inputPlayer in inputOnPlayer)
                {
                    ref var inputPlayerComponent = ref inputPlayer.GetComponent<InputComponent>();
                    
                    if (inputClientComponent.OwnerId == inputPlayerComponent.OwnerId)
                    {
                        inputPlayerComponent.PlayerInput = inputClientComponent.PlayerInput;
                    } 
                }
            }
        }

        public void OnAwake()
        {
        }
    }
}