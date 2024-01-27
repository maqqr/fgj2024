using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace DOTSInMars.Buildings
{
    public class BuildingCatalogAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject _miner;
        [SerializeField] private GameObject _refinery;
        [SerializeField] private GameObject _manufacturer;
        [SerializeField] private GameObject _conveyor;

        class BuildingCatalogBaker : Baker<BuildingCatalogAuthoring>
        {
            public override void Bake(BuildingCatalogAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new BuildingCatalog
                {
                    Miner = GetEntity(authoring._miner, TransformUsageFlags.None),
                    Refinery = GetEntity(authoring._refinery, TransformUsageFlags.None),
                    Manufacturer = GetEntity(authoring._manufacturer, TransformUsageFlags.None),
                    Conveyor = GetEntity(authoring._conveyor, TransformUsageFlags.None),
                });
            }
        }
    }

    public struct BuildingCatalog : IComponentData
    {
        public Entity Miner;
        public Entity Refinery;
        public Entity Manufacturer;
        public Entity Conveyor;
    }
}
