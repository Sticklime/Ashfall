using CodeBase.GameLogic.Common;
using Unity.Entities;
using Unity.NetCode;
using Cysharp.Threading.Tasks;
using CodeBase.Infrastructure.Factory;
using UnityEngine;
using VContainer;

[DisableAutoCreation]
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ServerSimulation)]
[UpdateInGroup(typeof(GhostSimulationSystemGroup))]
public partial class PlayerSpawnSystem : SystemBase
{
    [Inject] private IGameFactory _factory;
    private bool _spawned;

    protected override void OnUpdate()
    {
        if (_spawned)
            return;
        
        if (World.IsServer())
        {
            foreach (var (networkId, entity) in
                     SystemAPI.Query<RefRO<NetworkId>>().WithEntityAccess().WithNone<PlayerTag>())
            {
                SpawnAsync(true, networkId.ValueRO.Value).Forget();
                _spawned = true;
                break;
            }
        }
        else if (World.IsClient())
        {
            if (!SystemAPI.QueryBuilder().WithAll<NetworkStreamConnection>().Build().IsEmpty)
            {
                SpawnAsync(false).Forget();
                _spawned = true;
            }
        }
    }

    private async UniTaskVoid SpawnAsync(bool isServer, int ownerNetworkId = -1)
    {
        await _factory.CreatePlayer(isServer ? ownerNetworkId : -1);

        Debug.Log(isServer
            ? "[DOTS NET] Player spawned on SERVER"
            : "[DOTS NET] Player spawned on CLIENT");
    }
}