using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Array2DEditor;

namespace DOTSInMars
{
    [System.Serializable]
    public class ObstaclePrefabInfo
    {
        public GameObject Prefab;
        public Array2DBool BlockerInfo;
    }

    public struct ObstaclePrefabElement : IBufferElementData
    {
        public Entity Value;
        public FixedList64Bytes<bool> BlockedCells;
        public int BlockerSizeX;
        public int BlockerSizeZ;
        public bool GetBlockerAt(int x, int y)
        {
            int index = x * BlockerSizeX + y;
            return BlockedCells[index];
        }
    }

    public struct ObstaclePrefabSingleton : IComponentData
    {
    }

    public class ObstaclePrefabsAuthoring : MonoBehaviour
    {
        public List<ObstaclePrefabInfo> ObstaclePrefabList;
    }

    public class ObstaclePrefabsBaker : Baker<ObstaclePrefabsAuthoring>
    {
        public override void Bake(ObstaclePrefabsAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<ObstaclePrefabSingleton>(entity);
            DynamicBuffer<ObstaclePrefabElement> buffer = AddBuffer<ObstaclePrefabElement>(entity);
            foreach (ObstaclePrefabInfo prefabInfo in authoring.ObstaclePrefabList)
            {
                FixedList64Bytes<bool> blockers = new FixedList64Bytes<bool>();
                // get the selected blocks
                for (int x = 0; x < prefabInfo.BlockerInfo.GridSize.x; x++)
                {
                    for (int y = 0; y < prefabInfo.BlockerInfo.GridSize.y; y++)
                    {
                        var entry = prefabInfo.BlockerInfo.GetCell(x, y);
                        blockers.Add(entry);
                    }
                }

                var element = new ObstaclePrefabElement
                {
                    Value = GetEntity(prefabInfo.Prefab, TransformUsageFlags.None),
                    BlockedCells = blockers,
                    BlockerSizeX = prefabInfo.BlockerInfo.GridSize.x,
                    BlockerSizeZ = prefabInfo.BlockerInfo.GridSize.y,
                };
                buffer.Add(element);
            }
        }
    }
}