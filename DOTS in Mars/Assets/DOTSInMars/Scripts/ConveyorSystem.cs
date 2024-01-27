using System.Collections;
using System.Collections.Generic;
using DOTSInMars.Buildings;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace DOTSInMars
{
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
            }).WithAll<ConveyedItem, ResourceItem>().Run();

            Entities.ForEach((Entity entity, in LocalTransform localTransform) => {
                conveyors.Add(WorldGridUtils.ToGridPosition(localTransform.Position), entity);
            }).WithAll<Conveyor>().WithNone<BuildingPreviewTag>().Run();

            // Update conveyor progress and item positions
            Entities.ForEach((ref ConveyedItem conveyedItem, ref LocalTransform localTransform) => {
                conveyedItem.Progress = math.min(1.0f, conveyedItem.Progress + SystemAPI.Time.DeltaTime);

                float3 movement = WorldGridUtils.FromGridPosition(conveyedItem.TargetPosition - conveyedItem.StartPosition) * conveyedItem.Progress;
                localTransform.Position = new float3(0.5f, 0.3f, 0.5f) + WorldGridUtils.FromGridPosition(conveyedItem.StartPosition) + movement;
            }).Run();

            // Put items on conveyors if possible
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
            Entities.ForEach((Entity entity, in LocalTransform localTransform) => {
                int3 gridPosition = WorldGridUtils.ToGridPosition(localTransform.Position);
    
                if (conveyors.TryGetValue(gridPosition, out Entity conveyorEntity))
                {
                    if (!conveyedItems.TryGetValue(gridPosition, out Entity anotherItem))
                    {
                        ecb.SetComponentEnabled<ConveyedItem>(entity, true);
                        ecb.SetComponent(entity, new ConveyedItem { StartPosition = gridPosition, TargetPosition = gridPosition, Progress = 1.0f });
                        conveyedItems.Add(gridPosition, entity);
                    }
                }
            }).WithAll<ResourceItem>().WithDisabled<ConveyedItem>().Run();

            // Pass items onto next conveyor if movement has completed
            Entities.ForEach((Entity entity, ref ConveyedItem conveyedItem) => {
                if (conveyedItem.Progress >= 1.0f)
                {
                    if (conveyors.TryGetValue(conveyedItem.TargetPosition, out Entity conveyorEntity))
                    {
                        LocalTransform conveyorTransform = SystemAPI.GetComponent<LocalTransform>(conveyorEntity);

                        int3 conveyorGridPosition = WorldGridUtils.ToGridPosition(conveyorTransform.Position);
                        int3 conveyorTargetGridPosition = WorldGridUtils.ToGridPosition(conveyorTransform.Position + conveyorTransform.TransformDirection(new float3(1, 0, 0)));

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
            }).WithAll<ConveyedItem>().Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}