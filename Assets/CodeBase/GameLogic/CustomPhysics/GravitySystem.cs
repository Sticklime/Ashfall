using CodeBase.GameLogic.Common;
using CodeBase.GameLogic.Movement;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace CodeBase.GameLogic.CustomPhysics
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class GravitySystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (physics, groundCheck, transform, controller) in
                     SystemAPI.Query<RefRW<PhysicsComponent>, RefRO<GroundCheckComponent>, RefRW<LocalTransform>, ManagedAPI<CharacterControllerComponent>>()
                         .WithAll<PlayerTag>())
            {
                CharacterController characterController = controller.Value.Controller;
                if (characterController == null)
                    continue;

                if (!groundCheck.ValueRO.IsGrounded)
                {
                    physics.ValueRW.Velocity += math.down() * physics.ValueRO.Weight * deltaTime;
                    Vector3 moveVector = new Vector3(0f, physics.ValueRO.Velocity.y * deltaTime, 0f);
                    characterController.Move(moveVector);
                }
                else
                {
                    physics.ValueRW.Velocity.y = 0f;
                    float targetY = groundCheck.ValueRO.GroundPoint.y + groundCheck.ValueRO.CheckGroundDistance;
                    Vector3 controllerPosition = characterController.transform.position;
                    controllerPosition.y = targetY;
                    characterController.transform.position = controllerPosition;
                }

                Vector3 currentPosition = characterController.transform.position;
                transform.ValueRW.Position = new float3(currentPosition.x, currentPosition.y, currentPosition.z);
            }
        }
    }
}