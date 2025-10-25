using System;
using Fusion;
using UnityEngine;

namespace CodeBase.GameLogic
{
    public class NetworkSync : NetworkBehaviour
    {
        public event Action OnServerUpdate;

        public override void FixedUpdateNetwork()
        {
            OnServerUpdate?.Invoke();
            Debug.Log("Work");
        }
    }
}