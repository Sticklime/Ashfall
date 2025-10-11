using Fusion;
using Scellecs.Morpeh;

namespace CodeBase.GameLogic.Input
{
    public class NetworkInputApplySystem : ISystem
    {
        public World World { get; set; }

        private NetworkRunner _runner;
        private Filter _filter;

        public NetworkInputApplySystem(NetworkRunner runner)
        {
            _runner = runner;
        }

        public void OnAwake()
        {
            _filter = World.Filter
                .With<InputComponent>()
                .Build();

        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref var inputComponent = ref entity.GetComponent<InputComponent>();
                
                inputComponent.PlayerInput = inputComponent.NetworkInputReceiver.PlayerInput;
            }
        }

        public void Dispose()
        {
        }
    }
}