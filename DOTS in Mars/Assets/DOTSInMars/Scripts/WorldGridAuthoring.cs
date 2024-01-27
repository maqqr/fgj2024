using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace DOTSInMars
{


    public class WorldGridCellAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject gridCellPrefab;
        class WorldGridCellBaker : Baker<WorldGridCellAuthoring>
        {
            public override void Bake(WorldGridCellAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new GridSpawner
                {
                    GridCellPrefab = GetEntity(authoring.gridCellPrefab, TransformUsageFlags.None)
                }
                );
            }
        }
        public struct GridSpawner : IComponentData
        {
            public Entity GridCellPrefab;
        }
    }
}