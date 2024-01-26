using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTSInMars.Buildings
{

    public partial class BuildingSpawnerSystem : SystemBase
    {
        private List<(float3, Entity)> _spawns = new List<(float3, Entity)> ();

        internal void RegisterMinerForAdding(float3 position)
        {
            var entity = SystemAPI.GetSingletonEntity<BuildingCatalog>();
            var data = EntityManager.GetComponentData<BuildingCatalog>(entity);

            _spawns.Add(new(position, data.Miner));
        }

        protected override void OnUpdate()
        {
            if (_spawns.Count == 0)
            {
                return;
            }
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            for (int i = 0; i < _spawns.Count; i++)
            {
                var (position, prefab) = _spawns[i];
                var newMiner = ecb.Instantiate(prefab);
                ecb.SetComponent(newMiner, new LocalTransform
                {
                    Position = position,
                });
            }
            _spawns.Clear();
            ecb.Playback(EntityManager);
        }
    }
}
