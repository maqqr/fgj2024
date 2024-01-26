using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct WorldGridCell : IComponentData
{
    public bool Blocked;
    public Vector3Int Coordinates;
}
