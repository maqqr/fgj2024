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
            Entity singleton = SystemAPI.GetSingletonEntity<ResourceItemPrefabSingleton>();

            if (SystemAPI.Time.ElapsedTime > lastSpawnTime + 3.0)
            {
                lastSpawnTime = SystemAPI.Time.ElapsedTime;

                DynamicBuffer<ResourceItemPrefabElement> buffer = SystemAPI.GetBuffer<ResourceItemPrefabElement>(singleton);
                Entity prefabEntity = ResourceItemPrefab.Get(buffer, Resources.ResourceType.Bronze);

                var itemEntity = EntityManager.Instantiate(prefabEntity);
                RefRW<LocalTransform> localTransform = SystemAPI.GetComponentRW<LocalTransform>(itemEntity);
                localTransform.ValueRW.Position = new float3(2.5f, 0.0f, 2.5f);
            }
        }
    }
}