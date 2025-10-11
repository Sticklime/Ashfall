using CodeBase.GameLogic.Common;
using CodeBase.GameLogic.Input;
using Scellecs.Morpeh;
using UnityEngine;

namespace CodeBase.GameLogic.Movement
{
    public class SprintSystem : ISystem
    {
        private Filter _moveFilter;
        public World World { get; set; }

        public void OnAwake()
        {
            _moveFilter = World.Filter
                .With<MoveComponent>()
                .With<InputComponent>()
                .With<PlayerTag>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _moveFilter)
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