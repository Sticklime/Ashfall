using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CodeBase.Config.Player
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Config/PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        [field: SerializeField] public AssetReferenceGameObject PlayerReference { get; private set; }
        [field: SerializeField] public float MoveSpeed { get; private set; }
        [field: SerializeField] public float SprintSpeed { get; private set; }
        [field: SerializeField] public float JumpForce { get; private set; }
        [field: SerializeField] public float Weight { get; private set; }
        [field: SerializeField] public float CheckGroundDistance { get; private set; }
        [field: SerializeField] public LayerMask GroundCheckLayer { get; private set; }
        [field: SerializeField] public float InteractDistance { get; private set; }
        [field: SerializeField] public LayerMask InteractMask { get; private set; }
        [field: SerializeField] public float CameraSensitivityDefault { get; private set; }
    }
}