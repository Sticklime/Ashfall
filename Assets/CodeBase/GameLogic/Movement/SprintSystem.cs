using CodeBase.GameLogic.Common;
using CodeBase.GameLogic.Input;
using Unity.Entities;

namespace CodeBase.GameLogic.Movement
{
    public partial class SprintSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var (move, input) in SystemAPI.Query<RefRW<MoveComponent>, RefRO<InputComponent>>().WithAll<PlayerTag>())
            {
                move.ValueRW.Speed = input.ValueRO.PlayerInput.SprintProgress
                    ? move.ValueRO.SprintSpeed
                    : move.ValueRO.SpeedBase;
            }
        }
    }
}