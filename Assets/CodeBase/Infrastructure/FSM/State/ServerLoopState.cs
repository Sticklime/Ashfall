using System;
using System.Collections.Generic;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.Services.Input;
using Fusion;
using Fusion.Sockets;
using MessagePipe;
using UnityEngine;
using Input = CodeBase.GameLogic.Input.Input;

namespace CodeBase.Infrastructure.FSM.State
{
    public class ServerLoopState : IState, INetworkRunnerCallbacks
    {
        private readonly IStateMachine _stateMachine;
        private readonly IGameFactory _gameFactory;
        private readonly IInputService _inputService;
        private readonly NetworkRunner _networkRunner;

        public ServerLoopState(IStateMachine stateMachine, IGameFactory gameFactory, IInputService inputService,
            NetworkRunner networkRunner)
        {
            _stateMachine = stateMachine;
            _gameFactory = gameFactory;
            _inputService = inputService;
            _networkRunner = networkRunner;
        }

        public void Enter()
        {
            _networkRunner.AddCallbacks(this);
        }

        public void Exit()
        {
            _networkRunner.RemoveCallbacks(this);
        }

        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public async void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            await _gameFactory.CreatePlayer(player);
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request,
            byte[] token)
        {
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key,
            ArraySegment<byte> data)
        {
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            var data = new Input
            {
                Move = _inputService.Move,
                Look = _inputService.Look,
                JumpTriggered = _inputService.JumpTriggered,
                SprintProgress = _inputService.SprintProgress
            };

            input.Set(data);
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
        }
    }
}