using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTSInMars
{
    public partial class BuildingSystem : SystemBase
    {
        double lastSpawnTime = 0.0f;

        protected override void OnUpdate()
        {
            Entity singleton = SystemAPI.GetSingletonEntity<ResourceItemPrefabs>();
            ResourceItemPrefabsAspect resourceItemPrefabs = SystemAPI.GetAspect<ResourceItemPrefabsAspect>(singleton);

            if (SystemAPI.Time.ElapsedTime > lastSpawnTime + 3.0)
            {
                lastSpawnTime = SystemAPI.Time.ElapsedTime;

                var itemEntity = EntityManager.Instantiate(resourceItemPrefabs.GetPrefab(Resources.ResourceType.Iron));
                RefRW<LocalTransform> localTransform = SystemAPI.GetComponentRW<LocalTransform>(itemEntity);
                localTransform.ValueRW.Position = new float3(2.5f, 0.0f, 2.5f);
            }
        }
    }
}