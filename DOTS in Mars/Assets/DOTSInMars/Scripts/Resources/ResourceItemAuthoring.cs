using DOTSInMars.Resources;
using Unity.Entities;
using UnityEngine;

namespace DOTSInMars
{
    public struct ResourceItem : IComponentData
    {
        public ResourceType Value;
    }

    public class ResourceItemAuthoring : MonoBehaviour
    {
        public ResourceType ResourceType;
    }

    public class ResourceItemBaker : Baker<ResourceItemAuthoring>
    {
        public override void Bake(ResourceItemAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ResourceItem { Value = authoring.ResourceType });
            AddComponent(entity, new ConveyedItem());
            SetComponentEnabled<ConveyedItem>(entity, false);
        }
    }
}