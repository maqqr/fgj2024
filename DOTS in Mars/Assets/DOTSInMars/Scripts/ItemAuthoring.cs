using Unity.Entities;
using UnityEngine;

public struct Item : IComponentData
{
}

public class ItemAuthoring : MonoBehaviour
{
}

public class ItemBaker : Baker<ItemAuthoring>
{
    public override void Bake(ItemAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new Item());
    }
}