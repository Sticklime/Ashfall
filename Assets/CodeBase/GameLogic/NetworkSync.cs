using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace CodeBase.GameLogic
{
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkSync : NetworkBehaviour
    {
        private List<INetworkFixedUpdateable> _networkFixedUpdateables = new();

        private void RegisterNetworkFixedUpdateable(INetworkFixedUpdateable networkFixedUpdateable)
        {
            if (_networkFixedUpdateables.Contains(networkFixedUpdateable))
                return;

            _networkFixedUpdateables.Add(networkFixedUpdateable);
        }

        public override void FixedUpdateNetwork()
        {
            foreach (INetworkFixedUpdateable networkFixedUpdateable in _networkFixedUpdateables)
                networkFixedUpdateable.OnFixedUpdateNetwork();

            Debug.Log("Work");
        }
    }

    public interface INetworkFixedUpdateable
    {
        void OnFixedUpdateNetwork();
    }
}