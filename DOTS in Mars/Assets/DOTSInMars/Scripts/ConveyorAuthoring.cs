using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct Conveyor : IComponentData
{
}

public class ConveyorAuthoring : MonoBehaviour
{
}

public class ConveyorBaker : Baker<ConveyorAuthoring>
{
    public override void Bake(ConveyorAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new Conveyor());
    }
}