using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace DOTSInMars
{
    public struct GridHighlight : IComponentData
    {
        public float3 Offset;
    }
}
