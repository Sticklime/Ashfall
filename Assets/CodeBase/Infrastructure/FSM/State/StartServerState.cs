using Fusion;

namespace CodeBase.Infrastructure.FSM.State
{
    public class StartServerState : IState
    {
        private readonly IStateMachine _stateMachine;
        private readonly NetworkRunner _networkRunner;
        private readonly IGameFactory _gameFactory;

        public StartServerState(IStateMachine stateMachine, NetworkRunner networkRunner, IGameFactory gameFactory)
        {
            _stateMachine = stateMachine;
            _networkRunner = networkRunner;
            _gameFactory = gameFactory;
        }

        public async void Enter()
        {
            var startGameArgs = new StartGameArgs
            {
                GameMode = GameMode.Host,
                SessionName = "DefaultRoom",
                PlayerCount = 2,
                SceneManager = _networkRunner.gameObject.AddComponent<NetworkSceneManagerDefault>()
            };

            var result = await _networkRunner.StartGame(startGameArgs);

            if (result.Ok)
                _stateMachine.Enter<LoadLevelState>();
            else
                UnityEngine.Debug.LogError($"Server start failed: {result.ShutdownReason}");
        }

        public void Exit() { }
    }
}