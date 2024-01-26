using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct ConveyedItem : IComponentData, IEnableableComponent
{
    public int3 startPosition;
    public int3 targetPosition;
    public float progress;
}

public partial class ConveyorSystem : SystemBase
{
    static int3 ToGridPosition(in float3 position)
    {
        return new int3
        {
            x = (int)math.floor(position.x),
            y = (int)math.floor(position.y),
            z = (int)math.floor(position.z),
        };
    }

    public static float3 FromGridPosition(in int3 gridPosition)
    {
        return new float3
        {
            x = gridPosition.x,
            y = gridPosition.y,
            z = gridPosition.z,
        };
    }

    protected override void OnUpdate()
    {
        var conveyedItems = new NativeHashMap<int3, Entity>(100, Allocator.Temp);
        var conveyors = new NativeHashMap<int3, Entity>(100, Allocator.Temp);

        Entities.ForEach((Entity entity, in ConveyedItem conveyedItem) => {
            conveyedItems.Add(conveyedItem.targetPosition, entity);
        }).WithAll<Item>().Run();

        Entities.ForEach((Entity entity, in LocalTransform localTransform) => {
            conveyors.Add(ToGridPosition(localTransform.Position), entity);
        }).WithAll<Conveyor>().Run();

        // Update conveyor progress and item positions
        Entities.ForEach((ref ConveyedItem conveyedItem, ref LocalTransform localTransform) => {
            conveyedItem.progress = math.min(1.0f, conveyedItem.progress + SystemAPI.Time.DeltaTime);

            float3 movement = FromGridPosition(conveyedItem.targetPosition - conveyedItem.startPosition) * conveyedItem.progress;
            localTransform.Position = new float3(0.5f, 0.4f, 0.5f) + FromGridPosition(conveyedItem.startPosition) + movement;
        }).Run();

        // Put items on conveyors if possible
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        Entities.ForEach((Entity entity, in LocalTransform localTransform) => {
            int3 gridPosition = ToGridPosition(localTransform.Position);
            Entity conveyorEntity;
 
            if (conveyors.TryGetValue(gridPosition, out conveyorEntity))
            {
                if (!conveyedItems.TryGetValue(gridPosition, out Entity anotherItem))
                {
                    ecb.SetComponentEnabled<ConveyedItem>(entity, true);
                    ecb.SetComponent(entity, new ConveyedItem { startPosition = gridPosition, targetPosition = gridPosition, progress = 1.0f });
                    conveyedItems.Add(gridPosition, entity);
                }
            }
        }).WithAll<Item>().WithNone<ConveyedItem>().Run();

        // Pass items onto next conveyor if movement has completed
        Entities.ForEach((Entity entity, ref ConveyedItem conveyedItem) => {
            if (conveyedItem.progress >= 1.0f)
            {
                if (conveyors.TryGetValue(conveyedItem.targetPosition, out Entity conveyorEntity))
                {
                    LocalTransform conveyorTransform = SystemAPI.GetComponent<LocalTransform>(conveyorEntity);

                    int3 conveyorGridPosition = ToGridPosition(conveyorTransform.Position);
                    int3 conveyorTargetGridPosition = ToGridPosition(conveyorTransform.Position + conveyorTransform.TransformDirection(new float3(1, 0, 0)));

                    if (!conveyedItems.TryGetValue(conveyorTargetGridPosition, out Entity anotherItem))
                    {
                        conveyedItem.startPosition = conveyorGridPosition;
                        conveyedItem.targetPosition = conveyorTargetGridPosition;
                        conveyedItem.progress = 0.0f;
                        conveyedItems.Add(conveyedItem.targetPosition, entity);
                    }
                }
            }
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
