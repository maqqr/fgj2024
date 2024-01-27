using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace DOTSInMars.Buildings
{
    // Building entities have this component during item production
    public struct BuildingProduction : IComponentData, IEnableableComponent
    {
        public double ProductionEndTime;
    }

    public struct Building : IComponentData
    {
        public BuildingType Type;
        public int Recipe;
        public float3 OutputOffset;
        public FixedList128Bytes<float3> InputOffsets;

        public FixedList64Bytes<int> ContainedItems;
    }
}
