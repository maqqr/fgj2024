using Array2DEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;


namespace DOTSInMars
{
    public struct Obstacle : IComponentData
    {

    }

    public class ObstacleAuthoring : MonoBehaviour
    {
    }

    public class ObstacleBaker : Baker<ObstacleAuthoring>
    {
        public override void Bake(ObstacleAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            var obstacle = new Obstacle();
            AddComponent(entity, obstacle);
        }
    }
}