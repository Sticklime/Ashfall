using CodeBase.GameLogic.Common;
using CodeBase.GameLogic.Movement;
using Unity.Entities;
using UnityEngine;

namespace CodeBase.GameLogic.PickUp
{
    public partial class InteractSystem : SystemBase
    {
        private RaycastHit[] _hits;

        protected override void OnCreate()
        {
            _hits = new RaycastHit[2];
        }

        protected override void OnDestroy()
        {
            _hits = null;
        }

        protected override void OnUpdate()
        {
            EntityManager entityManager = EntityManager;

            foreach (var (interactComponent, entity) in
                     SystemAPI.Query<RefRO<InteractComponent>>()
                         .WithAll<PlayerTag>()
                         .WithEntityAccess())
            {
                if (!entityManager.HasComponent<CameraComponent>(entity))
                    continue;

                var cameraComponent = entityManager.GetComponentData<CameraComponent>(entity);
                Camera camera = cameraComponent.Camera;
                if (camera == null)
                    continue;

                Vector3 origin = camera.transform.position;
                Vector3 direction = camera.transform.forward;

                int hitCount = Physics.RaycastNonAlloc(origin, direction, _hits,
                    interactComponent.ValueRO.InteractDistance,
                    interactComponent.ValueRO.InteractMask);

#if UNITY_EDITOR
                if (hitCount > 0)
                {
                    var hit = _hits[0];
                    Debug.DrawLine(origin, hit.point, Color.green);
                    Debug.Log($"[InteractSystem] Hit: {hit.collider.gameObject.name}");
                }
                else
                {
                    Debug.DrawRay(origin, direction * interactComponent.ValueRO.InteractDistance, Color.red);
                }
#endif
            }
        }
    }
}