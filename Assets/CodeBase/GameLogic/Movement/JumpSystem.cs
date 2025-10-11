using CodeBase.GameLogic.Common;
using CodeBase.GameLogic.Input;
using Scellecs.Morpeh;
using UnityEngine;

public class JumpSystem : ISystem
{
    private Filter _filter;
    public World World { get; set; }

    public void OnAwake()
    {
         _filter = World.Filter
            .With<CharacterControllerComponent>()
            .With<JumpComponent>()
            .With<PhysicsComponent>()
            .With<InputComponent>()
            .With<RigidbodyComponent>()
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
            ref var input = ref entity.GetComponent<InputComponent>();

            var characterController = controllerComponent.Controller;

            Debug.Log($"Jump Triggerd {input.PlayerInput.JumpTriggered}");
            Debug.Log($"Is Grounded {physicsComponent.IsGrounded}");
            if (input.PlayerInput.JumpTriggered && physicsComponent.IsGrounded)
                physicsComponent.Velocity.y = Mathf.Sqrt(jumpComponent.JumpForce * 2f * physicsComponent.Weight);

            Vector3 moveVector = new Vector3(0, physicsComponent.Velocity.y, 0);
            characterController.Move(moveVector * deltaTime);
        }
    }

    public void Dispose()
    {
    }
}