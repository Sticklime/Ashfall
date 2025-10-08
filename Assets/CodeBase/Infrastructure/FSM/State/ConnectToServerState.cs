
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
            await UniTask.Delay(2000);
            var startGameArgs = new StartGameArgs
            {
                GameMode = GameMode.Client,
                SessionName = "DefaultRoom",
                SceneManager = _networkRunner.gameObject.AddComponent<NetworkSceneManagerDefault>()
            };

            var result = await _networkRunner.StartGame(startGameArgs);

            if (result.Ok)
            {
                await UniTask.Delay(1000);
                _stateMachine.Enter<ClientLoopState>();
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