using CodeBase.GameLogic.Common;
using Scellecs.Morpeh;
using UnityEngine;

namespace CodeBase.GameLogic.Movement
{
    public class GravitySystem : IFixedSystem
    {
        private Filter _physicFilter;
        private readonly RaycastHit[] _hits = new RaycastHit[1];

        public World World { get; set; }

        public void OnAwake()
        {
            _physicFilter = World.Filter
                .With<PhysicsComponent>()
                .With<TransformComponent>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _physicFilter)
            {
                ref var physic = ref entity.GetComponent<PhysicsComponent>();
                ref var transform = ref entity.GetComponent<TransformComponent>();

                var transformRef = transform.Transform;
                bool isGrounded = TryGetGroundPoint(transformRef, physic.CheckGroundDistance, physic.LayerGround, out var groundPoint);

                if (!isGrounded)
                {
                    physic.Velocity += Vector3.down * physic.Weight * deltaTime;
                    transformRef.position += physic.Velocity * deltaTime;
                }
                else
                {
                    physic.Velocity = Vector3.zero;
                    
                    Vector3 targetPosition = new Vector3(
                        transformRef.position.x,
                        groundPoint.y + physic.CheckGroundDistance,
                        transformRef.position.z
                    );

                    transformRef.position = targetPosition;
                }

                physic.IsGrounded = isGrounded;

                Debug.DrawRay(transformRef.position, Vector3.down * physic.CheckGroundDistance,
                    isGrounded ? Color.green : Color.red);
            }
        }
        
        private bool TryGetGroundPoint(Transform transform, float checkGroundDistance, LayerMask layerGround, out Vector3 groundPoint)
        {
            float skinOffset = 0.1f;
            Vector3 origin = transform.position + Vector3.up * skinOffset;

            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, checkGroundDistance + skinOffset, layerGround))
            {
                groundPoint = hit.point;
                return true;
            }

            groundPoint = Vector3.zero;
            return false;
        }

        public void Dispose()
        {
            _physicFilter = null;
        }
    }
}
