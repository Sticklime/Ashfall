using Scellecs.Morpeh;
using UnityEngine;

namespace CodeBase.GameLogic.PickUp
{
    public struct InteractComponent : IComponent
    {
        public float InteractDistance;
        public LayerMask InteractMask;
    }
}