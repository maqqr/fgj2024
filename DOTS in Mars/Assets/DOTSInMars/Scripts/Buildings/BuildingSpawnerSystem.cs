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
        private class BuildingPreview
        {
            public float3 Position;
            public Entity Prefab;
            public quaternion Rotation;

            public BuildingPreview(float3 float3, Entity miner, quaternion rotation)
            {
                Position = float3;
                Prefab = miner;
                Rotation = rotation;
            }
        }

        private BuildingPreview _spawn;
        private Camera _camera;
        private bool _raycastRequested;
        private BuildingType _buildingType;
        private bool _onclick;
        private bool _placeable;

        public event Action BuildingSet;


        protected override void OnUpdate()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            if (_spawn != null)
            {
                HandleRaycasting();

                if (Input.GetKeyUp(KeyCode.E))
                {
                    //ROTATES BY RADIANS!.?!!
                    _spawn.Rotation = math.mul(quaternion.RotateY(-1.5708f), _spawn.Rotation);
                }
                if (Input.GetKeyUp(KeyCode.R))
                {
                    //ROTATES BY RADIANS!.?!!
                    _spawn.Rotation = math.mul(quaternion.RotateY(1.5708f), _spawn.Rotation);
                }

                //TODO: use UI click instead of this to allow for UI to take control from this
                if (_onclick)
                {
                    _onclick = false;
                    if (_placeable)
                    {
                        var shifted = Input.GetKey(KeyCode.LeftShift);
                        var building = SpawnBuildings();
                        var pos = EntityManager.GetComponentData<LocalTransform>(building);
                        var gridPosition = WorldGridUtils.ToGridPosition(pos.Position);

                        Entity foundGridEntity = default;
                        Entities.ForEach((Entity entity, ref LocalTransform transform, ref WorldGridCell cell) =>
                        {
                            if (math.all(cell.Coordinates == gridPosition))
                            {
                                foundGridEntity = entity;
                            }
                        }).WithoutBurst().Run();


                        var grid = EntityManager.GetComponentData<WorldGridCell>(foundGridEntity);
                        grid.Blocked = true;
                        EntityManager.SetComponentData(foundGridEntity, grid);

                        _placeable = false;
                       
                        if (!shifted)
                        {
                            _spawn = null;
                            var previewEntity = SystemAPI.GetSingletonEntity<BuildingPreviewTag>();
                            EntityManager.DestroyEntity(previewEntity);
                            BuildingSet?.Invoke();
                        }
                    }
                }
            }

            //TODO: dont use _spawn
            Entities.ForEach((ref LocalTransform trans, in BuildingPreviewTag preview) =>
            {
                trans.Position = _spawn.Position;
                trans.Rotation = _spawn.Rotation;

            }).WithoutBurst().Run();


            if (_raycastRequested)
            {
                var entity = SystemAPI.GetSingletonEntity<BuildingCatalog>();
                var data = EntityManager.GetComponentData<BuildingCatalog>(entity);

                var prefab = _buildingType switch
                {
                    BuildingType.Miner => data.Miner,
                    BuildingType.Refinery => data.Refinery,
                    BuildingType.Manufacturer => data.Manufacturer,
                    _ => data.Conveyor
                };

                _spawn = new(new float3(0, 0.5f, 0), prefab, quaternion.identity);

                var spawnedEntity = SpawnBuildings();
                _placeable = true;
                //TODO: add material that is transparent etc.
                EntityManager.AddComponent<BuildingPreviewTag>(spawnedEntity);

                _raycastRequested = false;
            }
        }

        private Entity SpawnBuildings()
        {
            var newMiner = EntityManager.Instantiate(_spawn.Prefab);
            var rwTrans = SystemAPI.GetComponentRW<LocalTransform>(newMiner);
            rwTrans.ValueRW.Position = _spawn.Position;
            rwTrans.ValueRW.Rotation = _spawn.Rotation;

            return newMiner;
        }

        private void HandleRaycasting()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            var rayStart = ray.origin;
            var rayEnd = ray.GetPoint(100f);


            if (!Raycast(rayStart, rayEnd, out Unity.Physics.RaycastHit hit))
            {
                return;
            }
            if (!EntityManager.HasComponent<WorldGridCell>(hit.Entity))
            {
                return;
            }
            var grid = EntityManager.GetComponentData<WorldGridCell>(hit.Entity);
            if (grid.Blocked)
            {
                //TODO: some red and error sounds
                return;
            }
            _placeable = true;
            _spawn.Position = new float3(grid.Coordinates.x + 0.5f, grid.Coordinates.y, grid.Coordinates.z + 0.5f);
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

        internal void RegisterRaycasting(BuildingType type)
        {
            _raycastRequested = true;
            _buildingType = type;

            if (_spawn != null)
            {
                var previewEntity = SystemAPI.GetSingletonEntity<BuildingPreviewTag>();
                EntityManager.DestroyEntity(previewEntity);
            }
        }

        internal void OnClick()
        {
            _onclick = true;
        }
    }
}
