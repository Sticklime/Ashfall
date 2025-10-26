using CodeBase.Infrastructure.Factory;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

namespace CodeBase.Infrastructure.FSM.State
{
    public class ConnectToServerState : IState
    {
        private readonly IStateMachine _stateMachine;
        private readonly IGameFactory _gameFactory;
        private readonly NetworkRunner _networkRunner;

        public ConnectToServerState(IStateMachine stateMachine, IGameFactory gameFactory, NetworkRunner networkRunner)
        {
            _stateMachine = stateMachine;
            _gameFactory = gameFactory;
            _networkRunner = networkRunner;
        }

        public async void Enter()
        {
            _stateMachine.Enter<ClientLoopState>();

            var startGameArgs = new StartGameArgs
            {
                GameMode = GameMode.Client,
                SessionName = "DefaultRoom",
                Scene = SceneRef.FromIndex(0),
                SceneManager = _networkRunner.gameObject.AddComponent<NetworkSceneManagerDefault>()
            };
            
            var result = await _networkRunner.StartGame(startGameArgs);

            if (result.Ok)
            {
            }
            else
            {
                Debug.LogError($"Connection failed: {result.ShutdownReason}");
            }
        }

        public void Exit()
        {
        }
    }
}