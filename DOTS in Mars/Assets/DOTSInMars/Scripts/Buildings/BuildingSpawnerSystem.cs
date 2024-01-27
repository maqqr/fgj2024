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

namespace DOTSInMars.Buildings
{
    public partial class BuildingSpawnerSystem : SystemBase
    {
        private List<(float3, Entity)> _spawns = new List<(float3, Entity)> ();
        private Camera _camera;
        private BuildPhysicsWorld _buildPhysicsWorld;
        private CollisionWorld _collisionWorld;
        private bool _raycasting;
        private bool _raycastRequested;

        public event Action BuildingSet;

        internal void RegisterMinerForAdding(float3 screenPosition)
        {
            UnityEngine.Debug.Log($"Raycasting at {screenPosition}");
            
        }

        protected override void OnUpdate()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            if (_raycasting && Input.GetMouseButtonUp(0))
            {
                HandleRaycasting();
            }

            if (_spawns.Count > 0)
            {
                SpawnBuildings();
            }
            if (_raycastRequested)
            {
                _raycasting = true;
                _raycastRequested = false;
            }
        }

        private void SpawnBuildings()
        {
            for (int i = 0; i < _spawns.Count; i++)
            {
                var (position, prefab) = _spawns[i];
                var newMiner = EntityManager.Instantiate(prefab);
                var rwTrans = SystemAPI.GetComponentRW<LocalTransform>(newMiner);
                rwTrans.ValueRW.Position = position;
            }
            _spawns.Clear();
        }

        private void HandleRaycasting()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            var rayStart = ray.origin;
            var rayEnd = ray.GetPoint(100f);


            if (!Raycast(rayStart, rayEnd, out Unity.Physics.RaycastHit hit))
            {
                UnityEngine.Debug.Log($"No hit");
                return;
            }
            UnityEngine.Debug.Log($"Hit at {hit}");
            var grid = EntityManager.GetComponentData<WorldGridCell>(hit.Entity);
            if (grid.Blocked)
            {
                //TODO: some red and error sounds
                return;
            }

            var entity = SystemAPI.GetSingletonEntity<BuildingCatalog>();
            var data = EntityManager.GetComponentData<BuildingCatalog>(entity);

            _spawns.Add(new(new float3(grid.Coordinates.x, 0.5f, grid.Coordinates.z), data.Miner));
            _raycasting = false;
            BuildingSet?.Invoke();
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

        internal void RegisterRaycasting()
        {
            _raycastRequested = true;
        }
    }
}
