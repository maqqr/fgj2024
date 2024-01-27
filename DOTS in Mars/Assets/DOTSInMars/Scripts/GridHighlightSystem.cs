using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

namespace DOTSInMars
{
    public partial class GridHighlightSystem : SystemBase
    {

        private Camera _camera;
        private bool _initialized;
        const float highlighterOffset = 1f;
        private Entity mainHighlighter;
        private FixedList512Bytes<Entity> secondaryHighlighters;


        protected override void OnUpdate()
        {
            if (!_initialized)
            {
                InitializeHighlighters();
            }
            if (_camera == null)
            {
                _camera = Camera.main;
            }
            if (HandleRaycasting(out float3 targetCenter))
            {
                var transform = SystemAPI.GetComponentRW<LocalTransform>(mainHighlighter);
                transform.ValueRW.Position = targetCenter + new float3(0, highlighterOffset, 0);
                for (int i = 0; i < secondaryHighlighters.Length; i++)
                {
                    var secondaryEntity = secondaryHighlighters[i];
                    var secondaryTransform = SystemAPI.GetComponentRW<LocalTransform>(secondaryEntity);
                    var secondaryInfo = SystemAPI.GetComponentRW<GridHighlight>(secondaryEntity);
                    secondaryTransform.ValueRW.Position = targetCenter + new float3(0, highlighterOffset, 0) + secondaryInfo.ValueRO.Offset;
                }
            }
        }

        private void InitializeHighlighters()
        {
            Entity highlighterPrefab = SystemAPI.GetSingleton<WorldGridCellAuthoring.GridSpawner>().GridHighlighterPrefab;
            mainHighlighter = EntityManager.Instantiate(highlighterPrefab);
            EntityManager.AddComponent<GridHighlight>(mainHighlighter);
            InstantiateHighlightHelpers();
            _initialized = true;
        }

        private bool HandleRaycasting(out float3 cellPosition)
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            var rayStart = ray.origin;
            var rayEnd = ray.GetPoint(100f);

            cellPosition = new float3();


            if (!Raycast(rayStart, rayEnd, out Unity.Physics.RaycastHit hit))
            {
                return false;
            }
            if (EntityManager.HasComponent<WorldGridCell>(hit.Entity))
            {
                var cell = SystemAPI.GetComponentRW<WorldGridCell>(hit.Entity);
                cellPosition = cell.ValueRW.GetWorldCoordinate();
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
                    BelongsTo = 5u,
                    CollidesWith = ~0u
                }
            };
            return collisionWorld.CastRay(raycastInput, out target);
        }


        private void InstantiateHighlightHelpers()
        {
            var singleton = SystemAPI.GetSingleton<WorldGridCellAuthoring.GridSpawner>();
            Entity highlighterPrefab = singleton.GridSecondaryHighlighterPrefab;
            FixedList512Bytes<Entity> secondaries = new FixedList512Bytes<Entity>();
            for (int x = 0; x < singleton.SecondaryHighlightersXLength; x++)
            {
                for (int y = 0; y < singleton.SecondaryHighlightersYLength; y++)
                {
                    int index = x * singleton.SecondaryHighlightersXLength + y;
                    var enabled = singleton.SecondaryHighlighterPositions[index];
                    if (enabled)
                    {
                        var highlightEntity = EntityManager.Instantiate(highlighterPrefab);
                        secondaryHighlighters.Add(highlightEntity);
                        EntityManager.AddComponent<GridHighlight>(highlightEntity);
                        var highLightComponent = SystemAPI.GetComponentRW<GridHighlight>(highlightEntity);
                        float xOffset = (float)x - (float)singleton.SecondaryHighlightersXLength / 2 + 0.5f;
                        float zOffset = (float)y - (float)singleton.SecondaryHighlightersYLength / 2 + 0.5f;
                        highLightComponent.ValueRW.Offset = new float3(xOffset, 0, zOffset);
                        secondaries.Add(highlightEntity);
                    }
                }
            }
            secondaryHighlighters = secondaries; // a bit useless step
        }

    }
}
