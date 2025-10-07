using CodeBase.GameLogic.Common;
using Scellecs.Morpeh;
using UnityEngine;

namespace CodeBase.GameLogic.Movement
{
    public class GravitySystem : ISystem
    {
        private Collider[] _result = new Collider[2];
        public World World { get; set; }

        public void Dispose()
        {
        }

        public void OnUpdate(float deltaTime)
        {
            var physicFilter = World.Filter
                .With<PhysicsComponent>()
                .With<TransformComponent>()
                .Build();

            foreach (var entity in physicFilter)
            {
                ref var physic = ref entity.GetComponent<PhysicsComponent>();
                ref var transform = ref entity.GetComponent<TransformComponent>();

                var transformRef = transform.Transform;
                bool isGrounded = IsGrounded(transformRef, physic.CheckGroundDistance, physic.LayerGround);

                if (!isGrounded)
                {
                    physic.Velocity += Vector3.down * physic.Gravity * deltaTime;
                    transformRef.position += physic.Velocity * deltaTime;
                }
                else
                {
                    physic.Velocity = Vector3.zero;
                }

                physic.IsGrounded = isGrounded;
            }
        }

        private bool IsGrounded(Transform transform, float checkGroundDistance, LayerMask layerGround)
        {
            return Physics.OverlapSphereNonAlloc(transform.position, 0.3f, _result, layerGround) > 0;
        }

        public void OnAwake()
        {
        }
    }
}