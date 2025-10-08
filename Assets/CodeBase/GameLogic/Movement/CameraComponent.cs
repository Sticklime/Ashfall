using Fusion;
using Scellecs.Morpeh;
using UnityEngine;

public struct CameraComponent : IComponent
{
    public Camera Camera;
    public PlayerRef Owned;
}