using UnityEngine;

public interface IInputService
{
    Vector2 Move { get; }
    Vector2 Look { get; }
    bool JumpTriggered { get; }
    bool SprintProgress { get; }
    void Initialize();
}

public class InputService : IInputService
{
    private GameInput _input;

    public Vector2 Move => _input.Player.Move.ReadValue<Vector2>();
    public bool JumpTriggered => _input.Player.Jump.triggered;
    public bool SprintProgress => _input.Player.Sprint.inProgress;
    public Vector2 Look => _input.Player.Look.ReadValue<Vector2>();

    public void Initialize()
    {
        _input = new GameInput();
        _input.Enable();
    }
}