using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

namespace DOTSInMars
{

    public partial class WorldGridSystem : SystemBase
    {
        const int worldSizeX = 100;
        const int worldSizeZ = 100;
        float4 groundColor = new float4(0.7264151f, 0.5234901f, 0.4008988f, 1.0f);
        private bool initialized;

        protected override void OnCreate()
        {
            RequireForUpdate<WorldGridCellAuthoring.GridSpawner>();
        }

        protected override void OnUpdate()
        {
            if (!initialized)
            {
                //state.RequireForUpdate<execute>();
                Entity gridPrefab = SystemAPI.GetSingleton<WorldGridCellAuthoring.GridSpawner>().GridCellPrefab;

                for (int x = -worldSizeX / 2; x < worldSizeX / 2; x++)
                {
                    for (int z = -worldSizeZ / 2; z < worldSizeZ / 2; z++)
                    {
                        Entity entity = EntityManager.Instantiate(gridPrefab);
                        EntityManager.AddComponent<WorldGridCell>(entity);
                        var cell = SystemAPI.GetComponentRW<WorldGridCell>(entity);
                        cell.ValueRW.Coordinates = new int3(x, 0, z);
                        var transform = SystemAPI.GetComponentRW<LocalTransform>(entity);
                        transform.ValueRW.Position = new Vector3(x + 0.5f, -0.5f, z + 0.5f);

                        var obstacle = CheckForObstacles(cell.ValueRW);
                        if (obstacle)
                        {
                        }


                        EntityManager.AddComponent<URPMaterialPropertyBaseColor>(entity);
                        var entityColor = SystemAPI.GetComponentRW<URPMaterialPropertyBaseColor>(entity);
                        bool isEvenTile = (x + z) % 2 == 0;
                        float tintValue = isEvenTile ? 1.0f : 0.7f;
                        float4 finalColor = obstacle ? new float4(1, 0, 0, 1) * tintValue : groundColor * tintValue;
                        entityColor.ValueRW.Value = finalColor;
                    }
                }
                initialized = true;
                Debug.Log("Created world grid");
            }
        }

        private bool CheckForObstacles(WorldGridCell cell)
        {
            var cellCoordinate = cell.GetWorldCoordinate();
            var rayStart = cellCoordinate + new float3(0, 100, 0);
            var rayEnd = rayStart + new float3(0, -10000, 0);

            if (!Raycast(rayStart, rayEnd, out Unity.Physics.RaycastHit hit))
            {
                return false;
            }
            if (EntityManager.HasComponent<Obstacle>(hit.Entity))
            {
                //Debug.Log(hit.Position);
                if (hit.Position.y - 0.3f < cellCoordinate.y)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public bool Raycast(float3 rayFrom, float3 rayTo, out Unity.Physics.RaycastHit target)
        {
            EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<PhysicsWorldSingleton>();

            EntityQuery singletonQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(builder);
            var collisionWorld = singletonQuery.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            singletonQuery.Dispose();

            var raycastInput = new RaycastInput
            {
                Start = rayFrom,
                End = rayTo,
                Filter = new CollisionFilter()
                {
                    BelongsTo = ~0u,
                    CollidesWith = ~0u, // all 1s, so all layers, collide with everything
                    GroupIndex = 0
                }
            };
            return collisionWorld.CastRay(raycastInput, out target);
        }
    }

}