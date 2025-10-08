using CodeBase.GameLogic.Common;
using CodeBase.GameLogic.Input;
using Scellecs.Morpeh;
using UnityEngine;

namespace CodeBase.GameLogic.Movement
{
    public class SprintSystem : ISystem
    {
        public World World { get; set; }

        public void OnAwake()
        {
        }

        public void OnUpdate(float deltaTime)
        {
            Filter moveFilter = World.Filter
                .With<MoveComponent>()
                .With<InputComponent>()
                .With<PlayerTag>()
                .Build();

            foreach (var entity in moveFilter)
            {
                ref var moveComponent = ref entity.GetComponent<MoveComponent>();
                ref var inputComponent = ref entity.GetComponent<InputComponent>();

                moveComponent.Speed =
                    inputComponent.PlayerInput.SprintProgress ? moveComponent.SprintSpeed : moveComponent.SpeedBase;
            }
        }

        public void Dispose()
        {
        }
    }
}