using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace CodeBase.Config.Items
{
    [CreateAssetMenu(fileName = "ItemConfig", menuName = "Config/ItemConfig")]
    public class ItemsConfig : ScriptableObject
    {
        [field: SerializeField] public AssetReferenceGameObject ItemReference { get; private set; }
        [field: SerializeField] public ItemType ItemType { get; private set; }
    }

    public enum ItemType
    {
        Default = 0,
    }
}