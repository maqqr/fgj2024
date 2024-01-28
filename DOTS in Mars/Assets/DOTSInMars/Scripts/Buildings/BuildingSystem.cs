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
        public event System.Action DepositedFinalItem;

        protected override void OnUpdate()
        {
            Entity singleton = SystemAPI.GetSingletonEntity<ResourceItemPrefabSingleton>();

            var items = new NativeHashMap<int3, Entity>(100, Allocator.Temp);

            Entities.ForEach((Entity entity, in LocalTransform localTransform) => {
                items.TryAdd(WorldGridUtils.ToGridPosition(localTransform.Position), entity);
            }).WithAll<ResourceItem>().Run();

            // Make inputs eat items
            Entities.ForEach((ref Building building, in LocalTransform localTransform) => {
                Recipe recipe = Recipes.Get(building.Recipe);

                // Make sure contained items list is the same length as recipe inputs
                while (building.ContainedItems.Length < recipe.Inputs.Length)
                    building.ContainedItems.Add(0);

                for (int i = 0; i < recipe.Inputs.Length; i++)
                {
                    int3 eatPosition = WorldGridUtils.ToGridPosition(localTransform.Position + localTransform.TransformDirection(building.InputOffsets[i]));

                    // {
                    //     float3 asd = WorldGridUtils.FromGridPosition(eatPosition);
                    //     Vector3 start = new Vector3(asd.x + 0.5f, asd.y, asd.z + 0.5f);
                    //     UnityEngine.Debug.DrawLine(start, start + new Vector3(0, 2, 0), Color.red, 1.0f, false);
                    // }

                    if (items.TryGetValue(eatPosition, out Entity itemEntity))
                    {
                        if (EntityManager.HasComponent<ResourceItem>(itemEntity))
                        {
                            ResourceItem item = EntityManager.GetComponentData<ResourceItem>(itemEntity);
                            if (item.Value == recipe.Inputs[i].ResourceType && building.ContainedItems[i] < recipe.Inputs[i].Amount)
                            {
                                building.ContainedItems[i]++;

                                DynamicBuffer<Child> buffer = EntityManager.GetBuffer<Child>(itemEntity);
                                foreach (Child child in buffer)
                                {
                                    EntityManager.DestroyEntity(child.Value);
                                }
                                EntityManager.DestroyEntity(itemEntity);
                            }
                        }
                    }
                }
            }).WithNone<BuildingPreviewTag>().WithStructuralChanges().WithoutBurst().Run();

            // Start item production
            Entities.ForEach((Entity entity, ref Building building, in LocalTransform localTransform) => {
                Recipe recipe = Recipes.Get(building.Recipe);

                bool hasRequiredItemsInside = true;

                if (recipe.Output != Resources.ResourceType.Score)
                {
                    for (int i = 0; i < recipe.Inputs.Length; i++)
                    {
                        if (building.ContainedItems[i] < recipe.Inputs[i].Amount)
                            hasRequiredItemsInside = false;
                    }
                }
                else
                {
                    // Special case for score buildings: start producing score as soon as any input has something in it
                    hasRequiredItemsInside = false;
                    for (int i = 0; i < recipe.Inputs.Length; i++)
                    {
                        if (building.ContainedItems[i] > 0)
                            hasRequiredItemsInside = true;
                    }
                }

                if (hasRequiredItemsInside)
                {
                    // Consume items
                    if (recipe.Output != Resources.ResourceType.Score)
                    {
                        for (int i = 0; i < recipe.Inputs.Length; i++)
                            building.ContainedItems[i] -= recipe.Inputs[i].Amount;
                    }
                    else
                    {
                        for (int i = 0; i < recipe.Inputs.Length; i++)
                        {
                            if (building.ContainedItems[i] > 0)
                            {
                                building.ContainedItems[i]--;
                                break;
                            }
                        }
                    }

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
                        if (recipe.Output == Resources.ResourceType.Score)
                        {
                            DepositedFinalItem();
                        }
                        else
                        {
                            // Spawn item
                            DynamicBuffer<ResourceItemPrefabElement> buffer = SystemAPI.GetBuffer<ResourceItemPrefabElement>(singleton);
                            Entity prefabEntity = ResourceItemPrefab.Get(buffer, recipe.Output);
                            var itemEntity = EntityManager.Instantiate(prefabEntity);
                            LocalTransform itemLocalTransform = EntityManager.GetComponentData<LocalTransform>(itemEntity);
                            itemLocalTransform.Position = targetPosition;
                            EntityManager.SetComponentData(itemEntity, itemLocalTransform);
                        }

                        EntityManager.SetComponentEnabled<BuildingProduction>(entity, false);
                    }
                }
            }).WithAll<BuildingProduction>().WithNone<BuildingPreviewTag>().WithStructuralChanges().WithoutBurst().Run();
        }
    }
}