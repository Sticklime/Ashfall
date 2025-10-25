using CodeBase.GameLogic.Common;
using Scellecs.Morpeh;
using UnityEngine;

namespace CodeBase.GameLogic.CustomPhysics
{
    public class GravitySystem : ISystem
    {
        private Filter _filter;
        public World World { get; set; }

        public void OnAwake()
        {
            _filter = World.Filter
                .With<PhysicsComponent>()
                .With<GroundCheckComponent>()
                .With<TransformComponent>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref var physics = ref entity.GetComponent<PhysicsComponent>();
                ref var groundCheck = ref entity.GetComponent<GroundCheckComponent>();
                ref var transform = ref entity.GetComponent<TransformComponent>();

                var transformRef = transform.Transform;

                if (!groundCheck.IsGrounded)
                {
                    physics.Velocity += Vector3.down * physics.Weight * deltaTime;
                    transformRef.position += physics.Velocity * deltaTime;
                }
                else
                {
                    physics.Velocity = Vector3.zero;

                    Vector3 targetPosition = new Vector3(
                        transformRef.position.x,
                        groundCheck.GroundPoint.y + groundCheck.CheckGroundDistance,
                        transformRef.position.z
                    );

                    transformRef.position = targetPosition;
                }
            }
        }

        public void Dispose()
        {
            _filter = null;
        }
    }
}