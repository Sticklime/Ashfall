using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

namespace CodeBase.GameLogic.Input
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(GhostInputSystemGroup))]
    public partial struct NetworkInputApplySystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (input, networkInput) in
                     SystemAPI.Query<RefRW<InputComponent>, RefRO<NetworkInputReceiverComponent>>())
            {
                input.ValueRW.PlayerInput = networkInput.ValueRO.PlayerInput;
            }
        }
    }
}