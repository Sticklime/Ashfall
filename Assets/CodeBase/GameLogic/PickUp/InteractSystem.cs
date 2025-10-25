using CodeBase.GameLogic.Common;
using CodeBase.GameLogic.Movement;
using Scellecs.Morpeh;
using UnityEngine;

namespace CodeBase.GameLogic.PickUp
{
    public class InteractSystem : ISystem
    {
        public World World { get; set; }

        private Filter _playerFilter;

        private RaycastHit[] _hits = new RaycastHit[2];

        public void OnAwake()
        {
            _playerFilter = World.Filter
                .With<PlayerTag>()
                .With<CameraComponent>()
                .With<InteractComponent>()
                .Build();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _playerFilter)
            {
                ref var transformComponent = ref entity.GetComponent<CameraComponent>();
                ref var interactComponent = ref entity.GetComponent<InteractComponent>();

                var origin = transformComponent.Camera.transform.position;
                var direction = transformComponent.Camera.transform.forward;

                int hitCount = UnityEngine.Physics.RaycastNonAlloc(origin, direction, _hits,
                    interactComponent.InteractDistance,
                    interactComponent.InteractMask);

                if (hitCount > 0)
                {
                    var hit = _hits[0];
                    Debug.DrawLine(origin, hit.point, Color.green);
                    Debug.Log($"[InteractSystem] Hit: {hit.collider.gameObject.name}");
                }
                else
                {
                    Debug.DrawRay(origin, direction * interactComponent.InteractDistance, Color.red);
                }
            }
        }

        public void Dispose()
        {
            _hits = null;
        }
    }
}