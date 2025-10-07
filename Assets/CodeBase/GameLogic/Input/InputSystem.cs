using CodeBase.GameLogic.Common;
using Fusion;
using Scellecs.Morpeh;

namespace CodeBase.GameLogic.Input
{
    public class InputSystem : ISystem
    {
        private readonly IInputService _inputService;
        private readonly NetworkRunner _networkRunner;
        public World World { get; set; }

        public InputSystem(IInputService inputService, NetworkRunner networkRunner)
        {
            _inputService = inputService;
            _networkRunner = networkRunner;
        }

        public void OnAwake()
        {
        }

        public void OnUpdate(float deltaTime)
        {
            var filter = World.Filter
                .With<InputComponent>()
                .Build();

            foreach (var entity in filter)
            {
                ref var inputComponent = ref entity.GetComponent<InputComponent>();

                inputComponent.PlayerInput.Move = _inputService.Move;
                inputComponent.PlayerInput.Look = _inputService.Look;
                inputComponent.PlayerInput.JumpTriggered = _inputService.JumpTriggered;
                inputComponent.PlayerInput.SprintProgress = _inputService.SprintProgress;
            }
        }

        public void Dispose()
        {
        }
    }
}