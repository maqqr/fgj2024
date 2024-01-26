using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct ConveyedItem : IComponentData, IEnableableComponent
{
    public int3 StartPosition;
    public int3 TargetPosition;
    public float Progress;
}

public partial class ConveyorSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var conveyedItems = new NativeHashMap<int3, Entity>(100, Allocator.Temp);
        var conveyors = new NativeHashMap<int3, Entity>(100, Allocator.Temp);

        Entities.ForEach((Entity entity, in ConveyedItem conveyedItem) => {
            conveyedItems.Add(conveyedItem.TargetPosition, entity);
        }).WithAll<Item>().Run();

        Entities.ForEach((Entity entity, in LocalTransform localTransform) => {
            conveyors.Add(GridUtils.ToGridPosition(localTransform.Position), entity);
        }).WithAll<Conveyor>().Run();

        // Update conveyor progress and item positions
        Entities.ForEach((ref ConveyedItem conveyedItem, ref LocalTransform localTransform) => {
            conveyedItem.Progress = math.min(1.0f, conveyedItem.Progress + SystemAPI.Time.DeltaTime);

            float3 movement = GridUtils.FromGridPosition(conveyedItem.TargetPosition - conveyedItem.StartPosition) * conveyedItem.Progress;
            localTransform.Position = new float3(0.5f, 0.4f, 0.5f) + GridUtils.FromGridPosition(conveyedItem.StartPosition) + movement;
        }).Run();

        // Put items on conveyors if possible
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        Entities.ForEach((Entity entity, in LocalTransform localTransform) => {
            int3 gridPosition = GridUtils.ToGridPosition(localTransform.Position);
            Entity conveyorEntity;
 
            if (conveyors.TryGetValue(gridPosition, out conveyorEntity))
            {
                if (!conveyedItems.TryGetValue(gridPosition, out Entity anotherItem))
                {
                    ecb.SetComponentEnabled<ConveyedItem>(entity, true);
                    ecb.SetComponent(entity, new ConveyedItem { StartPosition = gridPosition, TargetPosition = gridPosition, Progress = 1.0f });
                    conveyedItems.Add(gridPosition, entity);
                }
            }
        }).WithAll<Item>().WithNone<ConveyedItem>().Run();

        // Pass items onto next conveyor if movement has completed
        Entities.ForEach((Entity entity, ref ConveyedItem conveyedItem) => {
            if (conveyedItem.Progress >= 1.0f)
            {
                if (conveyors.TryGetValue(conveyedItem.TargetPosition, out Entity conveyorEntity))
                {
                    LocalTransform conveyorTransform = SystemAPI.GetComponent<LocalTransform>(conveyorEntity);

                    int3 conveyorGridPosition = GridUtils.ToGridPosition(conveyorTransform.Position);
                    int3 conveyorTargetGridPosition = GridUtils.ToGridPosition(conveyorTransform.Position + conveyorTransform.TransformDirection(new float3(1, 0, 0)));

                    if (!conveyedItems.TryGetValue(conveyorTargetGridPosition, out Entity anotherItem))
                    {
                        conveyedItem.StartPosition = conveyorGridPosition;
                        conveyedItem.TargetPosition = conveyorTargetGridPosition;
                        conveyedItem.Progress = 0.0f;
                        conveyedItems.Add(conveyedItem.TargetPosition, entity);
                    }
                }
                else
                {
                    ecb.SetComponentEnabled<ConveyedItem>(entity, false);
                }
            }
        }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
