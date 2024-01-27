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
        public FixedList512Bytes<bool> BlockedCells;
        public int BlockerSizeX;
        public int BlockerSizeZ;
        public bool GetBlockerAt(int x, int y)
        {
            Debug.Log(x + " - " + y);
            int index = x * (BlockerSizeX - 1) + y;
            return BlockedCells[index];
        }
    }

    public class ObstacleAuthoring : MonoBehaviour
    {
        public Array2DBool BlockerInfo;
    }

    public class ObstacleBaker : Baker<ObstacleAuthoring>
    {
        public override void Bake(ObstacleAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            FixedList64Bytes<bool> blockers = new FixedList64Bytes<bool>();
            // get the selected blocks
            for (int x = 0; x < authoring.BlockerInfo.GridSize.x; x++)
            {
                for (int y = 0; y < authoring.BlockerInfo.GridSize.y; y++)
                {
                    var entry = authoring.BlockerInfo.GetCell(x, y);
                    blockers.Add(entry);
                }
            }
            var obstacle = new Obstacle
            {
                BlockedCells = blockers,
                BlockerSizeX = authoring.BlockerInfo.GridSize.x,
                BlockerSizeZ = authoring.BlockerInfo.GridSize.y,
            };
            AddComponent(entity, obstacle);
        }
    }
}