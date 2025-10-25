using CodeBase.GameLogic.Common;
using Scellecs.Morpeh;
using UnityEngine;

namespace CodeBase.GameLogic.CustomPhysics
{
    public class GroundCheckSystem : ISystem
    {
        private Filter _filter;
        public World World { get; set; }

        public void OnAwake()
        {
            _filter = World.Filter
                .With<GroundCheckComponent>()
                .With<TransformComponent>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref var groundCheck = ref entity.GetComponent<GroundCheckComponent>();
                ref var transform = ref entity.GetComponent<TransformComponent>();

                var transformRef = transform.Transform;
                bool isGrounded = TryGetGroundPoint(transformRef, groundCheck.CheckGroundDistance, groundCheck.LayerGround, out var groundPoint);

                groundCheck.IsGrounded = isGrounded;
                groundCheck.GroundPoint = groundPoint;

                Debug.DrawRay(transformRef.position, Vector3.down * groundCheck.CheckGroundDistance,
                    isGrounded ? Color.green : Color.red);
            }
        }

        private bool TryGetGroundPoint(Transform transform, float checkGroundDistance, LayerMask layerGround, out Vector3 groundPoint)
        {
            float skinOffset = 0.1f;
            Vector3 origin = transform.position + Vector3.up * skinOffset;

            if (UnityEngine.Physics.Raycast(origin, Vector3.down, out RaycastHit hit, checkGroundDistance + skinOffset, layerGround))
            {
                groundPoint = hit.point;
                return true;
            }

            groundPoint = Vector3.zero;
            return false;
        }

        public void Dispose()
        {
            _filter = null;
        }
    }
}