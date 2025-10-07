using System.ComponentModel;
using UnityEngine;
using IComponent = Scellecs.Morpeh.IComponent;

namespace CodeBase.GameLogic.Common
{
    public struct TransformComponent : IComponent
    {
        public Transform Transform;
    }
}