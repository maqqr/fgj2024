using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace DOTSInMars
{
    public struct WorldGridCell : IComponentData
    {
        public bool Blocked;
        public int3 Coordinates;

        // basically the transform
        public float3 GetWorldCoordinate()
        {
            return new float3(Coordinates.x + 0.5f, Coordinates.y - 0.5f, Coordinates.z + 0.5f);
        }
    }

    [MaterialProperty("_MainColor")]
    public struct GridColor : IComponentData
    {
        public float4 Value;
    }
}