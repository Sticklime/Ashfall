using CodeBase.Infrastructure.Services.Input;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace CodeBase.GameLogic.Input
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    public partial class ClientInputSystem : SystemBase
    {
        private static IInputService _inputService;

        protected override void OnCreate()
        {
            RequireForUpdate<NetworkStreamInGame>();
        }

        protected override void OnUpdate()
        {
            if (_inputService == null)
                return;

            var networkTime = SystemAPI.GetSingleton<NetworkTime>();

            foreach (var (commandBuffer, entity) in
                     SystemAPI.Query<RefRW<CommandTarget>>()
                         .WithEntityAccess())
            {
                if (commandBuffer.ValueRO.targetEntity == Entity.Null)
                    continue;

                var commandEntity = commandBuffer.ValueRO.targetEntity;
                if (!SystemAPI.HasBuffer<PlayerCommand>(commandEntity))
                    continue;

                var commandBufferData = SystemAPI.GetBuffer<PlayerCommand>(commandEntity);
                var input = _inputService;

                var command = new PlayerCommand
                {
                    Move = input.Move,
                    Look = new Vector3(input.Look.x, input.Look.y, 0f),
                    JumpTriggered = input.JumpTriggered,
                    SprintProgress = input.SprintProgress,
                    Tick = networkTime.ServerTick
                };

                commandBufferData.Add(command);
            }
        }
    }
}
