using Fusion;
using Unity.Entities;

namespace CodeBase.GameLogic.Input
{
    public class NetworkInputReceiver : NetworkBehaviour
    {
        public Input PlayerInput { get; private set; }

        private PlayerRef _playerRef;
        private Entity _entity = Entity.Null;
        private World _world;

        public void Construct(PlayerRef playerRef)
        {
            _playerRef = playerRef;
        }

        public void BindEntity(Entity entity, World world)
        {
            _entity = entity;
            _world = world;
        }

        public override void FixedUpdateNetwork()
        {
            if (Runner.IsServer && Runner.TryGetInputForPlayer<Input>(_playerRef, out var input))
            {
                PlayerInput = input;

                if (_world != null && _entity != Entity.Null)
                {
                    var entityManager = _world.EntityManager;

                    if (entityManager.Exists(_entity) && entityManager.HasComponent<NetworkInputReceiverComponent>(_entity))
                    {
                        entityManager.SetComponentData(_entity, new NetworkInputReceiverComponent
                        {
                            PlayerInput = PlayerInput
                        });
                    }
                }
            }
        }
    }
}