using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;

public partial struct WorldGridSystem : ISystem
{
    const int worldSizeX = 100;
    const int worldSizeZ = 100;
    private bool initialized;
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<WorldGridCellAuthoring.GridSpawner>();
    }
    public void OnDestroy(ref SystemState state)
    {

    }
    public void OnUpdate(ref SystemState state) 
    {
        if (!initialized)
        {
            //state.RequireForUpdate<execute>();
            Entity gridPrefab = SystemAPI.GetSingleton<WorldGridCellAuthoring.GridSpawner>().GridCellPrefab;

            for (int x = -worldSizeX / 2; x < worldSizeX / 2; x++)
            {
                for (int z = -worldSizeZ / 2; z < worldSizeZ / 2; z++)
                {
                    Entity entity = state.EntityManager.Instantiate(gridPrefab);
                    state.EntityManager.AddComponent<WorldGridCell>(entity);
                    var cell = SystemAPI.GetComponentRW<WorldGridCell>(entity);
                    cell.ValueRW.Coordinates = new Vector3Int(x, 0, z);
                    var transform = SystemAPI.GetComponentRW<LocalTransform>(entity);
                    transform.ValueRW.Position = new Vector3(x, 0, z);
                }
            }
            initialized = true;
            Debug.Log("Created world grid");
        }
    }
}
