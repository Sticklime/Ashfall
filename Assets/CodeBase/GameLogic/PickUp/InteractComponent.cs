using Unity.Entities;
using UnityEngine;

namespace CodeBase.GameLogic.PickUp
{
    public struct InteractComponent : IComponentData
    {
        public float InteractDistance;
        public LayerMask InteractMask;
    }
}