using Unity.Entities;
using UnityEngine;

namespace CodeBase.GameLogic.Movement
{
    public class CharacterControllerComponent : IComponentData
    {
        public CharacterController Controller;
    }
}