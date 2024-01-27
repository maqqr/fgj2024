using System.Collections;
using System.Collections.Generic;
using DOTSInMars.Buildings;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTSInMars
{
    public partial class BuildingSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entity singleton = SystemAPI.GetSingletonEntity<ResourceItemPrefabSingleton>();

            var items = new NativeHashMap<int3, Entity>(100, Allocator.Temp);

            Entities.ForEach((Entity entity, in LocalTransform localTransform) => {
                items.Add(WorldGridUtils.ToGridPosition(localTransform.Position), entity);
            }).WithAll<ResourceItem>().Run();

            // TODO eat items nearby

            // Start item production
            Entities.ForEach((Entity entity, ref Building building, in LocalTransform localTransform) => {
                Recipe recipe = Recipes.Get(building.Recipe);

                bool hasRequiredItemsInside = true;
                for (int i = 0; i < recipe.Inputs.Length; i++)
                {
                    if (i >= building.ContainedItems.Length || building.ContainedItems[i] < recipe.Inputs[i].Amount)
                        hasRequiredItemsInside = false;
                }
                if (hasRequiredItemsInside)
                {
                    // Consume items
                    for (int i = 0; i < recipe.Inputs.Length; i++)
                        building.ContainedItems[i] -= recipe.Inputs[i].Amount;

                    // Start production
                    EntityManager.SetComponentEnabled<BuildingProduction>(entity, true);
                    EntityManager.SetComponentData(entity, new BuildingProduction { ProductionEndTime = SystemAPI.Time.ElapsedTime + recipe.Duration });
                }
            }).WithNone<BuildingPreviewTag>().WithDisabled<BuildingProduction>().WithoutBurst().Run();

            // Finish production and spawn item
            Entities.ForEach((Entity entity, ref BuildingProduction buildingProduction, ref Building building, in LocalTransform localTransform) => {
                Recipe recipe = Recipes.Get(building.Recipe);
                if (SystemAPI.Time.ElapsedTime >= buildingProduction.ProductionEndTime)
                {
                    float3 targetPosition = localTransform.Position + localTransform.TransformDirection(building.OutputOffset);
                    int3 targetGridPosition = WorldGridUtils.ToGridPosition(targetPosition);
                    if (!items.TryGetValue(targetGridPosition, out Entity blockingItem))
                    {
                        // Spawn item
                        DynamicBuffer<ResourceItemPrefabElement> buffer = SystemAPI.GetBuffer<ResourceItemPrefabElement>(singleton);
                        Entity prefabEntity = ResourceItemPrefab.Get(buffer, recipe.Output);
                        var itemEntity = EntityManager.Instantiate(prefabEntity);
                        LocalTransform itemLocalTransform = EntityManager.GetComponentData<LocalTransform>(itemEntity);
                        itemLocalTransform.Position = targetPosition;
                        EntityManager.SetComponentData(itemEntity, itemLocalTransform);

                        EntityManager.SetComponentEnabled<BuildingProduction>(entity, false);
                    }
                }
            }).WithAll<BuildingProduction>().WithNone<BuildingPreviewTag>().WithStructuralChanges().WithoutBurst().Run();
        }
    }
}