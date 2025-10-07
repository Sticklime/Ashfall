using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CodeBase.Config.Player
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Config/PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        [field: SerializeField] public AssetReferenceGameObject PlayerReference { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject PlayerInputObjectReference { get; private set; }
    }
}