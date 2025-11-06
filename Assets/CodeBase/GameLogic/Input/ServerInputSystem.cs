using CodeBase.GameLogic.Common;
using Unity.Entities;
using Unity.NetCode;

namespace CodeBase.GameLogic.Input
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(GhostInputSystemGroup))]
    public partial class ServerInputSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var (commandBuffer, input) in
                     SystemAPI.Query<RefRO<CommandTarget>, RefRW<InputComponent>>()
                         .WithAll<PlayerTag>())
            {
                var commandEntity = commandBuffer.ValueRO.targetEntity;
                if (commandEntity == Entity.Null)
                    continue;

                if (!SystemAPI.HasBuffer<PlayerCommand>(commandEntity))
                    continue;

                var commandBufferData = SystemAPI.GetBuffer<PlayerCommand>(commandEntity);
                if (commandBufferData.Length == 0)
                    continue;

                var command = commandBufferData[commandBufferData.Length - 1];

                input.ValueRW.PlayerInput = new PlayerCommand
                {
                    Move = command.Move,
                    Look = command.Look,
                    JumpTriggered = command.JumpTriggered,
                    SprintProgress = command.SprintProgress,
                    Tick = command.Tick
                };
            }
        }
    }
}