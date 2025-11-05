using Unity.Entities;
using Unity.NetCode;

namespace CodeBase.GameLogic.Input
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(GhostInputSystemGroup))]
    public partial class NetworkInputApplySystem : SystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var (input, networkInput) in
                     SystemAPI.Query<RefRW<InputComponent>, RefRO<NetworkInputReceiverComponent>>())
            {
                input.ValueRW.PlayerInput = networkInput.ValueRO.PlayerInput;
            }
        }
    }
}