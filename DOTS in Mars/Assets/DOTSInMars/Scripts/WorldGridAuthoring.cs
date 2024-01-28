using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Array2DEditor;
using Unity.Collections;

namespace DOTSInMars
{


    public class WorldGridCellAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject gridCellPrefab;
        [SerializeField] private GameObject depositPrefab;
        [SerializeField] private GameObject gridHiglighterPrefab;
        [SerializeField] private GameObject gridSecondaryHiglighterPrefab;
        [SerializeField] private Array2DBool secondaryHighlighterPositions;
        class WorldGridCellBaker : Baker<WorldGridCellAuthoring>
        {
            public override void Bake(WorldGridCellAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                FixedList512Bytes<bool> highlighterPositions = new FixedList512Bytes<bool>();
                // get the selected blocks
                for (int x = 0; x < authoring.secondaryHighlighterPositions.GridSize.x; x++)
                {
                    for (int y = 0; y < authoring.secondaryHighlighterPositions.GridSize.y; y++)
                    {
                        var entry = authoring.secondaryHighlighterPositions.GetCell(x, y);
                        highlighterPositions.Add(entry);
                    }
                }
                AddComponent(entity, new GridSpawner
                {
                    GridCellPrefab = GetEntity(authoring.gridCellPrefab, TransformUsageFlags.None),
                    DepositPrefab = GetEntity(authoring.depositPrefab, TransformUsageFlags.None),
                    GridHighlighterPrefab = GetEntity(authoring.gridHiglighterPrefab, TransformUsageFlags.None),
                    GridSecondaryHighlighterPrefab = GetEntity(authoring.gridSecondaryHiglighterPrefab, TransformUsageFlags.None),
                    SecondaryHighlighterPositions = highlighterPositions,
                    SecondaryHighlightersXLength = authoring.secondaryHighlighterPositions.GridSize.x,
                    SecondaryHighlightersYLength = authoring.secondaryHighlighterPositions.GridSize.y
                });
            }
        }
        public struct GridSpawner : IComponentData
        {
            public Entity GridCellPrefab;
            public Entity DepositPrefab;
            public Entity GridHighlighterPrefab;
            public Entity GridSecondaryHighlighterPrefab;
            public FixedList512Bytes<bool> SecondaryHighlighterPositions;
            public int SecondaryHighlightersXLength;
            public int SecondaryHighlightersYLength;
        }
    }
}