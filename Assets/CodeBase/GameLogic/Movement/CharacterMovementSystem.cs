using CodeBase.GameLogic.Common;
using Scellecs.Morpeh;
using UnityEngine;

public class CharacterMovementSystem : ISystem
{
    public World World { get; set; }

    public void OnAwake()
    {
    }

    public void OnUpdate(float deltaTime)
    {
        var filter = World.Filter
            .With<MoveComponent>()
            .With<CharacterControllerComponent>()
            .With<TransformComponent>()
            .With<PlayerTag>()
            .Build();

        foreach (var entity in filter)
        {
            ref var moveComponent = ref entity.GetComponent<MoveComponent>();
            ref var controllerComponent = ref entity.GetComponent<CharacterControllerComponent>();
            ref var transformComponent = ref entity.GetComponent<TransformComponent>();
            ref var inputComponent = ref entity.GetComponent<InputComponent>();
            
            Vector2 moveInput = inputComponent.PlayerInput.Move;
            Transform transform = transformComponent.Transform;

            Vector3 direction = transform.right * moveInput.x + transform.forward * moveInput.y;
            Vector3 movement = direction.normalized * moveComponent.Speed * deltaTime;

            controllerComponent.Controller.Move(movement);
        }
    }

    public void Dispose()
    {
    }
}