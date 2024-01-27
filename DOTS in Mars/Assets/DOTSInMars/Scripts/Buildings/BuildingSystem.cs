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

            // TODO eat items nearby

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
            }).WithNone<BuildingProduction, BuildingPreviewTag>().WithoutBurst().Run();

            Entities.ForEach((ref BuildingProduction buildingProduction, ref Building building, in LocalTransform localTransform) => {
                Recipe recipe = Recipes.Get(building.Recipe);
                if (buildingProduction.ProductionEndTime >= SystemAPI.Time.ElapsedTime)
                {
                    // Spawn item
                    DynamicBuffer<ResourceItemPrefabElement> buffer = SystemAPI.GetBuffer<ResourceItemPrefabElement>(singleton);
                    Entity prefabEntity = ResourceItemPrefab.Get(buffer, Resources.ResourceType.Bronze);
                    var itemEntity = EntityManager.Instantiate(prefabEntity);
                    LocalTransform itemLocalTransform = EntityManager.GetComponentData<LocalTransform>(itemEntity);
                    itemLocalTransform.Position = localTransform.Position + localTransform.TransformDirection(building.OutputOffset);
                    EntityManager.SetComponentData(itemEntity, itemLocalTransform);
                }
            }).WithNone<BuildingPreviewTag>().WithStructuralChanges().WithoutBurst().Run();
        }
    }
}