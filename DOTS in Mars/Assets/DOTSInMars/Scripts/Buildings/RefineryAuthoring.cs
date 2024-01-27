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
    public class RefineryAuthoring : MonoBehaviour
    {
        class RefineryBaker : Baker<RefineryAuthoring>
        {
            public override void Bake(RefineryAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                var building = new Building
                {
                    Type = BuildingType.Refinery,
                    Recipe = 1,
                    OutputOffset = new float3(2.0f, 0.0f, 0.0f),
                };
                building.InputOffsets.Add(new float3(-2.0f, 0.0f, 0.0f));
                AddComponent(entity, building);
                AddComponent(entity, new BuildingProduction());
                SetComponentEnabled<BuildingProduction>(entity, false);
            }
        }
    }
}
