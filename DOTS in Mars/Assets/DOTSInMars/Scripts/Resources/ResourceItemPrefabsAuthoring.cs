using System.Collections.Generic;
using DOTSInMars.Resources;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

readonly partial struct ResourceItemPrefabsAspect : IAspect
{
    private readonly RefRW<ResourceItemPrefabs> resourceItemPrefabs;

    public Entity GetPrefab(ResourceType resourceType)
    {
        //return resourceItemPrefabs.ValueRO.ItemPrefabs.ElementAt((int)resourceType);
        switch (resourceType)
        {
            case ResourceType.Bronze: return resourceItemPrefabs.ValueRO.BronzeItemPrefab;
            case ResourceType.Iron: return resourceItemPrefabs.ValueRO.IronItemPrefab;
        }
        return Entity.Null;
    }
}

[InternalBufferCapacity(16)]
public struct ResourceItemPrefabElement : IBufferElementData
{
    public Entity Value;
}

public struct ResourceItemPrefabs : IComponentData
{
    //public FixedList64Bytes<Entity> ItemPrefabs;

    public Entity BronzeItemPrefab;
    public Entity IronItemPrefab;
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
        var data = new ResourceItemPrefabs();
        data.BronzeItemPrefab = GetEntity(authoring.ResourceItemPrefabList[0], TransformUsageFlags.None);
        data.IronItemPrefab = GetEntity(authoring.ResourceItemPrefabList[1], TransformUsageFlags.None);
        // foreach (GameObject gameObject in authoring.ResourceItemPrefabList)
        // {
        //     data.ItemPrefabs.Add(GetEntity(gameObject, TransformUsageFlags.None));
        // }
        AddComponent(entity, data);
    }
}