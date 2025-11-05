using CodeBase.GameLogic.Common;
using CodeBase.GameLogic.Input;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace CodeBase.GameLogic.Movement
{
    public partial class CharacterMovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (move, input, controller, transform) in
                     SystemAPI.Query<RefRW<MoveComponent>, RefRO<InputComponent>, ManagedAPI<CharacterControllerComponent>, RefRW<LocalTransform>>()
                         .WithAll<PlayerTag>())
            {
                Vector2 moveInput = input.ValueRO.PlayerInput.Move;

                if (moveInput.sqrMagnitude <= math.EPSILON)
                    continue;

                float3 forward = math.mul(transform.ValueRO.Rotation, new float3(0f, 0f, 1f));
                float3 right = math.mul(transform.ValueRO.Rotation, new float3(1f, 0f, 0f));

                float3 direction = math.normalizesafe(right * moveInput.x + forward * moveInput.y);
                float3 movement = direction * move.ValueRO.Speed * deltaTime;
                movement.y = 0f;

                CharacterController characterController = controller.Value.Controller;
                characterController.Move(new Vector3(movement.x, movement.y, movement.z));

                Vector3 controllerPosition = characterController.transform.position;
                transform.ValueRW.Position = new float3(controllerPosition.x, controllerPosition.y, controllerPosition.z);
            }
        }
    }
}