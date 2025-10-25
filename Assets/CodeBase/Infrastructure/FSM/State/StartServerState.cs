using CodeBase.Infrastructure.Factory;
using Fusion;
using UnityEngine;

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
            var networkSceneManagerDefault = _networkRunner.gameObject.AddComponent<NetworkSceneManagerDefault>();
            
            var startGameArgs = new StartGameArgs
            {
                GameMode = GameMode.Host,
                SessionName = "DefaultRoom",
                PlayerCount = 5,
                SceneManager = networkSceneManagerDefault
            };

            var result = await _networkRunner.StartGame(startGameArgs);

            if (result.Ok)
            {
                Debug.Log("Server started");
                _stateMachine.Enter<LoadLevelState>();
            }
            else
                Debug.LogError($"Server start failed: {result.ErrorMessage}");
        }

        public void Exit()
        {
        }
    }
}