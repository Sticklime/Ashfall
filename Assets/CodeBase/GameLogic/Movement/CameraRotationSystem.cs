using CodeBase.GameLogic.Common;
using CodeBase.GameLogic.Input;
using Scellecs.Morpeh;
using UnityEngine;

public class CameraRotationSystem : ISystem
{
    private Filter _filter;
    public World World { get; set; }

    public void OnAwake()
    {
        _filter = World.Filter
            .With<CameraRotationComponent>()
            .With<TransformComponent>()
            .With<InputComponent>()
            .With<CameraComponent>()
            .With<PlayerTag>()
            .Build();
    }

    public void OnUpdate(float deltaTime)
    {
        foreach (var entity in _filter)
        {
            ref var rotation = ref entity.GetComponent<CameraRotationComponent>();
            ref var camera = ref entity.GetComponent<CameraComponent>();
            ref var mount = ref entity.GetComponent<TransformComponent>();
            ref var input = ref entity.GetComponent<InputComponent>();

            Vector2 lookInput = input.PlayerInput.Look;

            float horizontalRotation = lookInput.x * rotation.Sensitivity * deltaTime;
            float verticalRotation = -lookInput.y * rotation.Sensitivity * deltaTime;

            rotation.VerticalAngle += verticalRotation;
            rotation.HorizontalAngle += horizontalRotation;
            rotation.VerticalAngle = Mathf.Clamp(rotation.VerticalAngle, -90f, 90f);

            camera.Camera.transform.localEulerAngles = new Vector3(rotation.VerticalAngle, 0f, 0f);
            mount.Transform.localEulerAngles = new Vector3(0f, rotation.HorizontalAngle, 0f);
        }
    }


    public void Dispose()
    {
    }
}