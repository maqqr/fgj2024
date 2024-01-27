using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTSInMars.Buildings
{
    public class MinerAuthoring : MonoBehaviour
    {
        class MinerBaker : Baker<MinerAuthoring>
        {
            public override void Bake(MinerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                var building = new Building
                {
                    Type = BuildingType.Miner,
                    Recipe = 0,
                    OutputOffset = new float3(2.0f, 0.0f, 1.0f),
                };
                AddComponent(entity, building);
                AddComponent(entity, new BuildingProduction());
                SetComponentEnabled<BuildingProduction>(entity, false);
            }
        }
    }
}
