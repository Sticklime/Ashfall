using Cysharp.Threading.Tasks;
using Fusion;

namespace CodeBase.Infrastructure.FSM.State
{
    public class LoadLevelState : IState
    {
        private readonly IStateMachine _stateMachine;
        private readonly IGameFactory _gameFactory;
        private readonly NetworkRunner _networkRunner;

        public LoadLevelState(IStateMachine stateMachine, IGameFactory gameFactory, NetworkRunner networkRunner)
        {
            _stateMachine = stateMachine;
            _gameFactory = gameFactory;
            _networkRunner = networkRunner;
        }

        public void Enter()
        {
            var localPlayer = _networkRunner.LocalPlayer;
            _gameFactory.CreatePlayer(localPlayer.PlayerId);
            _gameFactory.CreateInputEntity(localPlayer.PlayerId);

            _stateMachine.Enter<ServerLoopState>();
        }

        private async UniTask CreatePlayer(PlayerRef player)
        {
            await UniTask.Delay(5000);
            _gameFactory.CreatePlayer(player.PlayerId);
        }

        public void Exit()
        {
        }
    }
}