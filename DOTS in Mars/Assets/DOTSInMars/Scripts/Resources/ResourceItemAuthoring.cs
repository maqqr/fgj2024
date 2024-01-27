using DOTSInMars.Resources;
using Unity.Entities;
using UnityEngine;

public struct ResourceItem : IComponentData
{
    ResourceType Value;
}

public class ResourceItemAuthoring : MonoBehaviour
{
}

public class ResourceItemBaker : Baker<ResourceItemAuthoring>
{
    public override void Bake(ResourceItemAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new ResourceItem());
        AddComponent(entity, new ConveyedItem());
        SetComponentEnabled<ConveyedItem>(entity, false);
    }
}