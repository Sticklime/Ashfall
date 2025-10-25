using Fusion;

namespace CodeBase.GameLogic.Input
{
    public class NetworkInputReceiver : NetworkBehaviour
    {
        public Input PlayerInput { get; private set; }

        private PlayerRef _playerRef;

        public void Construct(PlayerRef playerRef)
        {
            _playerRef = playerRef;
        }

        public override void FixedUpdateNetwork()
        {
            if (Runner.IsServer && Runner.TryGetInputForPlayer<Input>(_playerRef, out var input))
            {
                PlayerInput = input;
            
            }
        }
    }
}