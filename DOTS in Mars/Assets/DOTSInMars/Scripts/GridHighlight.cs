using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace DOTSInMars
{
    public struct GridHighlight : IComponentData
    {
        public float3 Offset;
    }

    [MaterialProperty("_Offset")]
    public struct GridHighlightOffset : IComponentData
    {
        public float Value;
    }
    
    [MaterialProperty("_Multiplier")]
    public struct GridHighlightMultiplier : IComponentData
    {
        public float Value;
    }
    [System.Serializable]
    [MaterialProperty("_MainColor")]
    public struct GridHighlightColor : IComponentData
    {
        public float4 Value;
    }
}
