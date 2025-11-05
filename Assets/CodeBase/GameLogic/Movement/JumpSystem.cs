using CodeBase.GameLogic.Common;
using CodeBase.GameLogic.CustomPhysics;
using CodeBase.GameLogic.Input;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace CodeBase.GameLogic.Movement
{
    public partial class JumpSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (controller, jump, physics, groundCheck, input, transform) in
                     SystemAPI.Query<ManagedAPI<CharacterControllerComponent>, RefRO<JumpComponent>, RefRW<PhysicsComponent>, RefRO<GroundCheckComponent>, RefRO<InputComponent>, RefRW<LocalTransform>>()
                         .WithAll<PlayerTag>())
            {
                if (input.ValueRO.PlayerInput.JumpTriggered && groundCheck.ValueRO.IsGrounded)
                {
                    float jumpVelocity = math.sqrt(math.max(0f, jump.ValueRO.JumpForce * 2f * physics.ValueRO.Weight));
                    physics.ValueRW.Velocity.y = jumpVelocity;
                }

                float3 velocity = physics.ValueRO.Velocity;

                CharacterController characterController = controller.Value.Controller;
                Vector3 moveVector = new Vector3(0f, velocity.y, 0f) * deltaTime;
                characterController.Move(moveVector);

                Vector3 controllerPosition = characterController.transform.position;
                transform.ValueRW.Position = new float3(controllerPosition.x, controllerPosition.y, controllerPosition.z);
            }
        }
    }
}