using System.Collections.Generic;
using DOTSInMars.Resources;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace DOTSInMars
{
    public static class ResourceItemPrefab
    {
        public static Entity Get(in DynamicBuffer<ResourceItemPrefabElement> buffer, ResourceType resourceType)
        {
            return buffer[(int)resourceType].Value;
        }
    }

    [InternalBufferCapacity(16)]
    public struct ResourceItemPrefabElement : IBufferElementData
    {
        public Entity Value;
    }

    public struct ResourceItemPrefabSingleton : IComponentData
    {
    }

    public class ResourceItemPrefabsAuthoring : MonoBehaviour
    {
        public List<GameObject> ResourceItemPrefabList;
    }

    public class ResourceItemPrefabsBaker : Baker<ResourceItemPrefabsAuthoring>
    {
        public override void Bake(ResourceItemPrefabsAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<ResourceItemPrefabSingleton>(entity);
            DynamicBuffer<ResourceItemPrefabElement> buffer = AddBuffer<ResourceItemPrefabElement>(entity);
            foreach (GameObject gameObject in authoring.ResourceItemPrefabList)
            {
                buffer.Add(new ResourceItemPrefabElement { Value = GetEntity(gameObject, TransformUsageFlags.None) });
            }
        }
    }
}