using CodeBase.GameLogic.Common;
using CodeBase.GameLogic.CustomPhysics;
using CodeBase.GameLogic.Input;
using CodeBase.Infrastructure.Services.Input;
using Scellecs.Morpeh;
using UnityEngine;

namespace CodeBase.GameLogic.Movement
{
    public class JumpSystem : ISystem
    {
        private readonly IInputService _inputService;
        private Filter _filter;
        public World World { get; set; }

        public JumpSystem(IInputService inputService)
        {
            _inputService = inputService;
        }

        public void OnAwake()
        {
            _filter = World.Filter
                .With<CharacterControllerComponent>()
                .With<JumpComponent>()
                .With<PhysicsComponent>()
                .With<GroundCheckComponent>()
                .With<InputComponent>()
                .With<PlayerTag>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref var controllerComponent = ref entity.GetComponent<CharacterControllerComponent>();
                ref var jumpComponent = ref entity.GetComponent<JumpComponent>();
                ref var physicsComponent = ref entity.GetComponent<PhysicsComponent>();
                ref var groundCheckComponent = ref entity.GetComponent<GroundCheckComponent>();
                ref var input = ref entity.GetComponent<InputComponent>();

                var characterController = controllerComponent.Controller;

                if (input.PlayerInput.JumpTriggered && groundCheckComponent.IsGrounded)
                    physicsComponent.Velocity.y = Mathf.Sqrt(jumpComponent.JumpForce * 2f * physicsComponent.Weight);


                Vector3 moveVector = new Vector3(0, physicsComponent.Velocity.y, 0);
                characterController.Move(moveVector * deltaTime);
            }
        }

        public void Dispose()
        {
        }
    }
}