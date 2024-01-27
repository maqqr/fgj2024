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
        private class HighlightBlock
        {
            public float3 Position;
            public Entity Prefab;

            public HighlightBlock(float3 float3, Entity miner, quaternion rotation)
            {
                Position = float3;
                Prefab = miner;
            }
        }

        private Camera _camera;
        private bool _raycastRequested;
        private bool _initialized;

        const float highlighterOffset = 1f;

        private Entity mainHighlighter;


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
            }
        }

        private void InitializeHighlighters()
        {

            //state.RequireForUpdate<execute>();
            Entity highlighterPrefab = SystemAPI.GetSingleton<WorldGridCellAuthoring.GridSpawner>().GridHighlighterPrefab;

            mainHighlighter = EntityManager.Instantiate(highlighterPrefab);
            EntityManager.AddComponent<GridHighlight>(mainHighlighter);
            //var mainHighlighter = SystemAPI.GetComponentRW<GridHighlight>(entity);
            //mainHighlighterTransform = SystemAPI.GetComponentRW<LocalTransform>(entity).ValueRW;
            //_mainHighlight
            Debug.Log("Highlighter init");
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


        private void InstantiateHighlightHelper()
        {

        }
    }
}
