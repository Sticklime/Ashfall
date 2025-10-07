using CodeBase.GameLogic.Common;
using Scellecs.Morpeh;
using UnityEngine;

public class JumpSystem : ISystem
{
    public World World { get; set; }

    public JumpSystem()
    {
    }

    public void OnAwake()
    {
    }

    public void OnUpdate(float deltaTime)
    {
        var filter = World.Filter
            .With<CharacterControllerComponent>()
            .With<JumpComponent>()
            .With<PhysicsComponent>()
            .With<InputComponent>()
            .With<PlayerTag>()
            .Build();

        foreach (var entity in filter)
        {
            ref var controllerComponent = ref entity.GetComponent<CharacterControllerComponent>();
            ref var jumpComponent = ref entity.GetComponent<JumpComponent>();
            ref var physicsComponent = ref entity.GetComponent<PhysicsComponent>();
            ref var input = ref entity.GetComponent<InputComponent>();

            var characterController = controllerComponent.Controller;

            if (input.PlayerInput.JumpTriggered && physicsComponent.IsGrounded)
                physicsComponent.Velocity.y = Mathf.Sqrt(jumpComponent.JumpForce * 2f * physicsComponent.Gravity);

            Vector3 moveVector = new Vector3(0, physicsComponent.Velocity.y, 0);
            characterController.Move(moveVector * deltaTime);
        }
    }

    public void Dispose()
    {
    }
}