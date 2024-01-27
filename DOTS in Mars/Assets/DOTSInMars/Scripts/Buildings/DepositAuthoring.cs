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
    public class DepositAuthoring : MonoBehaviour
    {
        class DepositBaker : Baker<DepositAuthoring>
        {
            public override void Bake(DepositAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                var building = new Building
                {
                    Type = BuildingType.Deposit,
                    Recipe = 3,
                    OutputOffset = new float3(0.0f, 0.0f, 0.0f),
                };
                building.InputOffsets.Add(new float3(-2.0f, 0.0f, -1.0f));
                building.InputOffsets.Add(new float3(-2.0f, 0.0f, 1.0f));

                building.InputOffsets.Add(new float3(2.0f, 0.0f, -1.0f));
                building.InputOffsets.Add(new float3(2.0f, 0.0f, 1.0f));

                building.InputOffsets.Add(new float3(-1.0f, 0.0f, 2.0f));
                building.InputOffsets.Add(new float3(1.0f, 0.0f, 2.0f));

                building.InputOffsets.Add(new float3(-1.0f, 0.0f, -2.0f));
                building.InputOffsets.Add(new float3(1.0f, 0.0f, -2.0f));
                AddComponent(entity, building);
                AddComponent(entity, new BuildingProduction());
                SetComponentEnabled<BuildingProduction>(entity, false);
            }
        }
    }
}
